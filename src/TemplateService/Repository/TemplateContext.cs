using Microsoft.EntityFrameworkCore;

namespace TemplateService.Repository
{
    //
    // TODO: Rename this class appropriately
    //
    public class TemplateContext : DbContext
    {
        public TemplateContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Add fluent modeling for database here...including seed data if any
        }
    }
}