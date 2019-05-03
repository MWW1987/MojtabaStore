using Microsoft.AspNetCore.Mvc;
using MojtabaStore.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojtabaStore.Web.ViewComponents
{
    public class CourseGroupComponent : ViewComponent
    {
        private readonly ICourseService courseService;

        public CourseGroupComponent(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("CourseGroup", courseService.GetAllGroup()));
        }

    }
}
