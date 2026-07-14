using Microsoft.AspNetCore.Mvc;
using TrelloClone.Business.Interfaces;
using TrelloClone.Core.Entities;

namespace TrelloClone.Web.Controllers
{
    public class BoardController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IColumnService _columnService;
        private readonly ICardService _cardService;

        public BoardController(IProjectService projectService, IColumnService columnService, ICardService cardService)
        {
            _projectService = projectService;
            _columnService = columnService;
            _cardService = cardService;
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

                var column = await _columnService.GetColumnByIdAsync(card.ColumnId);
                return RedirectToAction("Detail", new { id = column.ProjectId });
            }

            ViewBag.ColumnId = card.ColumnId;
            return View(card);
        }
    }
}