using Microsoft.AspNetCore.Mvc;
using TrelloClone.Business.Interfaces;
using TrelloClone.Core.Entities;
using Microsoft.AspNetCore.SignalR;
using TrelloClone.Web.Hubs;

namespace TrelloClone.Web.Controllers
{
    public class BoardController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IColumnService _columnService;
        private readonly ICardService _cardService;
        private readonly IHubContext<BoardHub> _hubContext;
        private readonly IAuditLogService _auditLogService;

        public BoardController(IProjectService projectService, 
                               IColumnService columnService, 
                               ICardService cardService,
                               IHubContext<BoardHub> hubContext,
                               IAuditLogService auditLogService)
        {
            _projectService = projectService;
            _columnService = columnService;
            _cardService = cardService;
            _hubContext = hubContext;
            _auditLogService = auditLogService;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return View(projects);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null) return NotFound();

            var columns = await _columnService.GetColumnsByProjectIdAsync(id);
            
            foreach(var col in columns)
            {
                col.Cards = (await _cardService.GetCardsByColumnIdAsync(col.Id)).ToList();
            }
            project.Columns = columns.ToList();

            ViewBag.RecentLogs = await _auditLogService.GetRecentLogsAsync(10);

            return View(project);
        }

        public class UpdateCardRequest
        {
            public int CardId { get; set; }
            public int NewColumnId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCardPosition([FromBody] UpdateCardRequest request)
        {
            var card = await _cardService.GetCardByIdAsync(request.CardId);
            
            if (card != null)
            {
                card.ColumnId = request.NewColumnId;
                await _cardService.UpdateCardAsync(card);

                await _auditLogService.LogActionAsync("Kart Taşındı", $"Bir görev yeni bir sütuna taşındı.", "Sistem Kullanıcısı", request.CardId);
                
                // SignalR Bildirimi
                var column = await _columnService.GetColumnByIdAsync(request.NewColumnId);
                
                // CS8602 Uyarısını çözen Null Kontrolü:
                if (column != null) 
                {
                    var projectId = column.ProjectId.ToString();
                    await _hubContext.Clients.Group(projectId)
                        .SendAsync("ReceiveCardMove", request.CardId, request.NewColumnId);
                }
                
                return Ok(); 
            }
            return BadRequest();
        }

        [HttpGet]
        public IActionResult CreateProject()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(Project project)
        {
            ModelState.Remove("Columns");

            if (ModelState.IsValid)
            {
                await _projectService.CreateProjectAsync(project);
                await _columnService.CreateColumnAsync(new Column { Title = "Yapılacaklar", Order = 1, ProjectId = project.Id });
                await _columnService.CreateColumnAsync(new Column { Title = "Devam Edenler", Order = 2, ProjectId = project.Id });
                await _columnService.CreateColumnAsync(new Column { Title = "Bitenler", Order = 3, ProjectId = project.Id });

                return RedirectToAction(nameof(Index));
            }
            
            return View(project);
        }

        [HttpGet]
        public IActionResult CreateCard(int columnId)
        {
            ViewBag.ColumnId = columnId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard(Card card)
        {
            ModelState.Remove("Column");

            if (ModelState.IsValid)
            {
                var existingCards = await _cardService.GetCardsByColumnIdAsync(card.ColumnId);
                card.Order = existingCards.Count() + 1;

                await _cardService.CreateCardAsync(card);
                await _auditLogService.LogActionAsync("Kart Oluşturuldu", $"'{card.Title}' adlı kart eklendi.", "Sistem Kullanıcısı", card.Id);

                var column = await _columnService.GetColumnByIdAsync(card.ColumnId);
                return RedirectToAction("Detail", new { id = column.ProjectId });
            }

            ViewBag.ColumnId = card.ColumnId;
            return View(card);
        }
    }
}