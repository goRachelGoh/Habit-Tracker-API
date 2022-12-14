
using Api.DataTransferObjects;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers;

[ApiController]
[Route("api/habit-tracker")]
public class HabitController : ControllerBase
{
    private readonly IHabitService habitService;
    private UserManager<IdentityUser> _userManager;
    public HabitController(IHabitService habitService, UserManager<IdentityUser> userManager)
    {
        this.habitService = habitService;
        _userManager = userManager;

    }

    [HttpPost]
    [Route("")]
    async public Task<ActionResult<Habit>> Post([FromBody] Habit habit)
    {
        habit.Id = Guid.NewGuid();
        // better way to achieve this?
        habit.ArchivedStatus = (ArchiveStatus)0;
        habit.CompletionStatus = false;
        habit.CreatedOn = DateTime.Now;

        // habit.IdentityUserID = IdentityUser.ID WHERE userInfo.username(COOKIE) == IdentityUser.UserName
        // EX) habit.IdentityUserID = "665cae34-6d38-42d2-98c9-2ba221957b5b";
        var cookieUserName = HttpContext.Request.Cookies["userInfo"];
        var userInfoObj = JsonConvert.DeserializeObject<UserClientInfo>(cookieUserName);
        var result = await this._userManager.FindByNameAsync(userInfoObj.Username);
        habit.IdentityUserID = result.Id;


        if (habit == null)
        {
            return BadRequest();
        }
        await this.habitService.AddHabit(habit);
        return Ok();
    }



}
