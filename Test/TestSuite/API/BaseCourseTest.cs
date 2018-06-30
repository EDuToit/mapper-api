﻿using System;
using Mapper_Api.Context;
using Mapper_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace TestSuite.API
{
    public class BaseCourseTest
    {
        // Testable
        protected CourseService courseService;

        protected CourseDb db;

        public BaseCourseTest()
        {
            var options = new DbContextOptionsBuilder<CourseDb>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new CourseDb(options);

            courseService = new CourseService(context);
        }
    }
}