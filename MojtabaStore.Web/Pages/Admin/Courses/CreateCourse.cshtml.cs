using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MojtabaStore.Core.Services.Interfaces;
using MojtabaStore.DataLayer.Entities.Course;
using System.Linq;

namespace MojtabaStore.Web.Pages.Admin.Courses
{
    public class CreateCourseModel : PageModel
    {
        private ICourseService courseService;

        public CreateCourseModel(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        [BindProperty]
        public Course Course { get; set; }

        public void OnGet()
        {
            var groups = courseService.GetGroupForManageCourse();
            ViewData["Groups"] = new SelectList(groups, "Value", "Text");

            var subGrous = courseService.GetSubGroupForManageCourse(int.Parse(groups.First().Value));
            ViewData["SubGroups"] = new SelectList(subGrous, "Value", "Text");

            var teachers = courseService.GetTeachers();
            ViewData["Teachers"] = new SelectList(teachers, "Value", "Text");

            var levels = courseService.GetLevels();
            ViewData["Levels"] = new SelectList(levels, "Value", "Text");

            var statues = courseService.GetStatues();
            ViewData["Statues"] = new SelectList(statues, "Value", "Text");
        }

        public IActionResult OnPost(IFormFile imgCourseUp, IFormFile demoUp)
        {
            if (!ModelState.IsValid)
                return Page();

            courseService.AddCourse(Course, imgCourseUp, demoUp);
            return RedirectToPage("Index");
        }
    }
}