using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MojtabaStore.Core.Convertors;
using MojtabaStore.Core.DTOs.Course;
using MojtabaStore.Core.Generator;
using MojtabaStore.Core.Security;
using MojtabaStore.Core.Services.Interfaces;
using MojtabaStore.DataLayer.Context;
using MojtabaStore.DataLayer.Entities.Course;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MojtabaStore.Core.Services
{
    public class CourseService : ICourseService
    {
        private readonly MojtabaStoreContext context;

        public CourseService(MojtabaStoreContext context)
        {
            this.context = context;
        }

        public int AddCourse(Course course, IFormFile imgCourse, IFormFile courseDemo)
        {
            course.CreateDate = DateTime.Now;
            course.CourseImageName = "no-photo.jpg";
            //TODO Check Image
            if (imgCourse != null && imgCourse.IsImage())
            {
                course.CourseImageName = NameGenerator.GenerateUniqueCode() + Path.GetExtension(imgCourse.FileName);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/image", course.CourseImageName);

                using (FileStream stream = new FileStream(imagePath, FileMode.Create))
                {
                    imgCourse.CopyTo(stream);
                }

                ImageConvertor imgResizer = new ImageConvertor();
                string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/thumb", course.CourseImageName);
                imgResizer.Image_resize(imagePath, thumbPath, 150);
            }

            if (courseDemo != null)
            {
                course.DemoFileName = NameGenerator.GenerateUniqueCode() + Path.GetExtension(courseDemo.FileName);
                string demoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/demoes", course.DemoFileName);
                using (FileStream stream = new FileStream(demoPath, FileMode.Create))
                {
                    courseDemo.CopyTo(stream);
                }
            }

            context.Add(course);
            context.SaveChanges();

            return course.CourseId;
        }

        public List<CourseGroup> GetAllGroup()
        {
            return context.CourseGroups.ToList();
        }

        public List<ShowCourseForAdminViewModel> GetcoursesForAdmin()
        {
            return context.Courses.Select(c => new ShowCourseForAdminViewModel()
            {
                CourseId = c.CourseId,
                ImageName = c.CourseImageName,
                Title = c.CourseTitle,
                EpisodeCount = c.CourseEpisodes.Count
            }).ToList();
        }

        public List<SelectListItem> GetGroupForManageCourse()
        {
            return context.CourseGroups.Where(c => c.ParentId == null).Select(c => new SelectListItem()
            {
                Text = c.GroupTitle,
                Value = c.GroupId.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetLevels()
        {
            return context.CourseLevels.Select(c => new SelectListItem()
            {
                Value = c.LevelId.ToString(),
                Text = c.LevelTitle
            }).ToList();
        }

        public List<SelectListItem> GetStatues()
        {
            return context.CourseStatuses.Select(c => new SelectListItem()
            {
                Value = c.StatusId.ToString(),
                Text = c.StatusTitle
            }).ToList();
        }

        public List<SelectListItem> GetSubGroupForManageCourse(int groupId)
        {
            return context.CourseGroups.Where(c => c.ParentId == groupId).Select(c => new SelectListItem()
            {
                Text = c.GroupTitle,
                Value = c.GroupId.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetTeachers()
        {
            return context.UserRoles.Where(c => c.RoleId == 2).Include(c => c.User).Select(c => new SelectListItem()
            {
                Value = c.UserId.ToString(),
                Text = c.User.UserName
            }).ToList();
        }
    }
}
