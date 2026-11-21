using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TelephoneDirectory.Data.AppDbContext;
using TelephoneDirectory.Models.contact;

namespace TelephoneDirectory.Pages;

public class IndexModel : PageModel
{
    public List<Contact> ContactList { get; set; } = new List<Contact>();

    public string SearchText { get; set; } = "";

    private AppDbContext databaseContext;

    public IndexModel(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }
    public string ConvertToShamsiDate(DateOnly? gregorianDate)
    {
        if (gregorianDate == null)
        {
            return "";
        }

        PersianCalendar calendar = new PersianCalendar();

        DateTime date = gregorianDate.Value.ToDateTime(TimeOnly.MinValue);

        int year = calendar.GetYear(date);
        int month = calendar.GetMonth(date);
        int day = calendar.GetDayOfMonth(date);

        return $"{year}/{month:00}/{day:00}";
    }    
    public void OnGet(string searchText)
    {
        SearchText = searchText;

        foreach (var contact in databaseContext.contats)
        {
            if (string.IsNullOrWhiteSpace(SearchText) ||
                contact.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                contact.Mobile.Contains(SearchText))
            {
                ContactList.Add(contact);
            }
        }
    }
}
