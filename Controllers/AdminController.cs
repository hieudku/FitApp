using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]  // Only admin can access these actions
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: Admin/Users
    public IActionResult Users()
    {
        var users = _userManager.Users.ToList();  // Retrieve all users
        return View(users);
    }

    // GET: Admin/DeleteUser/{id}
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);  // Find user by ID
        if (user == null)
        {
            return NotFound();  // User not found
        }

        return View(user);  // Pass user to view for confirmation
    }

    // POST: Admin/DeleteUser/{id}
    [HttpPost, ActionName("DeleteUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUserConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);  // Find user by ID
        if (user == null)
        {
            return NotFound();  // User not found
        }

        var result = await _userManager.DeleteAsync(user);  // Delete the user
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Users));  // Redirect to user list after successful deletion
        }

        // Handle deletion errors
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(user);  // Return the view with errors if any
    }
}
