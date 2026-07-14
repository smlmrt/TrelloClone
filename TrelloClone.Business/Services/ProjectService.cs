using TrelloClone.Business.Interfaces;
using TrelloClone.Core.Entities;
using TrelloClone.Core.Interfaces;
using TrelloClone.DataAccess.Context;

namespace TrelloClone.Business.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly TrelloCloneDbContext _context; // SaveChanges için

        public ProjectService(IRepository<Project> projectRepository, TrelloCloneDbContext context)
        {
            _projectRepository = projectRepository;
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _projectRepository.GetAllAsync();
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _projectRepository.GetByIdAsync(id);
        }

        public async Task CreateProjectAsync(Project project)
        {
            await _projectRepository.AddAsync(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _projectRepository.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProjectAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project != null)
            {
                _projectRepository.Delete(project);
                await _context.SaveChangesAsync();
            }
        }
    }
}