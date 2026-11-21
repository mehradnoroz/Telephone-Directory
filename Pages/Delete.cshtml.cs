
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TelephoneDirectory.Data.AppDbContext;
using TelephoneDirectory.Models.contact;

namespace TelephoneDirectory.Pages;

public class DeleteModel : PageModel
{
    private AppDbContext databaseContext;

    public Contact? Contact { get; set; }

    public DeleteModel(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public IActionResult OnGet(int id)
    {
        Contact = databaseContext.contats.FirstOrDefault(contact=>contact.Id==id);

        if (Contact == null)
        {
            return NotFound();
        }

        return Page();
    }

    public IActionResult OnPost(int id)
    {
        Contact = databaseContext.contats.FirstOrDefault(contact=>   contact.Id == id);

        if (Contact == null)
        {
            return NotFound();
        }

        databaseContext.contats.Remove(Contact);
        databaseContext.SaveChanges();

        return RedirectToPage("/Index");
    }
}
