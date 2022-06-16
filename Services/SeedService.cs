using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoviePro.Data;
using MoviePro.Models.Database;
using MoviePro.Models.Settings;

namespace MoviePro.Services
{
    public class SeedService
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedService(IOptions<AppSettings> appSettings,
                           ApplicationDbContext dbContext,
                           UserManager<IdentityUser> userManager,
                           RoleManager<IdentityRole> roleManager)
        {
            _appSettings = appSettings.Value;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task ManageDataAsync()
        {
            await UpdateDatabaseAsync();

            await SeedRolesAsync();

            await SeedUsersAsync();

            await SeedCollections();
        }

        private async Task UpdateDatabaseAsync()
        {
            //runs through mirgations to make sure db structurally is up to date
            await _dbContext.Database.MigrateAsync();
        }

        private async Task SeedRolesAsync()
        {
            //responsible for creating user record in ASP.net user table with all my info
            //as well as assign me to the role of administrator
            if (_dbContext.Roles.Any()) return;

            var adminRole = _appSettings.MovieProSettings.DefaultCredentials.Role;

            await _roleManager.CreateAsync(new IdentityRole(adminRole));

        }

        private async Task SeedUsersAsync()
        {
            if (_userManager.Users.Any()) return; //if any users, don't do anything

            var credentials = _appSettings.MovieProSettings.DefaultCredentials;//incoming 
            var newUser = new IdentityUser()
            {
                Email = credentials.Email,
                UserName = credentials.Email,
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(newUser, credentials.Password);
            await _userManager.AddToRoleAsync(newUser, credentials.Role);

        }
        private async Task SeedCollections()
        {
            if (_dbContext.Collection.Any()) return;

            _dbContext.Add(new Collection()
            {
                Name = _appSettings.MovieProSettings.DefaultCollection.Name,
                Description = _appSettings.MovieProSettings.DefaultCollection.Description
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}
