/***
 * Filename: CoursesController.cs
 * Author  : Eben du Toit, Duncan Tilley
 * Class   : CoursesController
 *
 *      API entry point for courses.
 ***/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapper_Api.Context;
using Mapper_Api.Models;
using Mapper_Api.Services;
using Mapper_Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mapper_Api.Controllers
{
    [Produces("application/json")]
    public class CoursesController : Controller
    {
        private readonly CourseDb _context;
        private LocationService locationService;

        public CoursesController(CourseDb context, LocationService locationService)
        {
            _context = context;
            this.locationService =  locationService;
        }

        // GET: api/users/{id}/courses
        [Route("api/users/{uid}/courses")]
        [HttpGet]
        public async Task<IActionResult> GetUserCourses([FromRoute] Guid uid)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!UserExists(uid)) {
                return NotFound();
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _context.Users
                    .Include(m => m.Courses)
                    .SingleOrDefaultAsync(m => m.UserID == uid);

            if (user == null) return NotFound();

            return Ok(user.Courses);
        }

        // POST: api/users/{id}/courses

        [Route("api/users/{uid}/courses")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostUserCourse([FromRoute] Guid uid,
                [FromBody] Course course)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!UserExists(uid)) {
                return NotFound();
            }

            course.UserId = uid;

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGolfCourse",
                    new {id = course.CourseId}, course);
        }

// GET: api/courses
        [Route("api/courses/test/{CourseID}")]
        [HttpGet]
        public async Task<IEnumerable<LiveLocation>> getLiveLocation([FromRoute]String CourseID)
        {
            return await locationService.getRecentPlayerLocation(CourseID); //TODO test this
        }


        // GET: api/courses
        [Route("api/courses")]
        [HttpGet]
        public async Task<IEnumerable<Course>> GetCoursesAsync(Double? lat, Double? lon, int limit = 10)
        {
            if(lat == null || lon == null ){
                return _context.Courses;
            }else{
                return await locationService.sortCourseByPosition(lat, lon, limit);     
            }
        }

        // GET: api/courses/{id}
        [Route("api/courses/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetCourse([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var golfCourse = await _context.Courses
                    .Include(m => m.Holes)
                    .Select(c => new CourseViewModel()
                    {
                        CourseId = c.CourseId,
                        CourseName = c.CourseName,
                        Elements = c.Elements.Where(p => p.ElementType == Element.ElementTypes.POINT && p.HoleId == null)
                        .Cast<Point>()
                        .Select(d => new PointViewModel() {
                            CourseId = d.CourseId,
                            ElementId = d.ElementId,
                            ElementType = d.ElementType,
                            GeoJson = d.GeoJson,
                            Info = d.Info,
                            PointType = d.PointType, 
                            HoleId = d.HoleId
                        } as ElementViewModel
                        ).Concat(
                            c.Elements.Where(q => q.ElementType == Element.ElementTypes.POLYGON && q.HoleId == null)
                            .Cast<Polygon>()
                            .Select(d => new PolygonViewModel() {
                                CourseId = d.CourseId,
                                ElementId = d.ElementId,
                                ElementType = d.ElementType,
                                GeoJson = d.GeoJson,
                                PolygonType = d.PolygonType,
                                HoleId = d.HoleId
                            } as ElementViewModel
                            )
                        ).ToList(),
                        UserId = c.UserId,
                        Holes = c.Holes, 
                        Info = c.Info
                    })
                    .SingleOrDefaultAsync(m => m.CourseId == id);

            if (golfCourse == null) return NotFound();

            return Ok(golfCourse);
        }

        // PUT: api/courses/{id}
        [Route("api/courses/{id}")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> PutCourse([FromRoute] Guid id,
                [FromBody] Course golfCourse)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != golfCourse.CourseId) return BadRequest();

            _context.Entry(golfCourse).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/courses/{id}
        [Route("api/courses/{id}")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteCourse([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var golfCourse = await _context.Courses
                    .SingleOrDefaultAsync(m => m.CourseId == id);

            if (golfCourse == null) return NotFound();

            _context.Courses.Remove(golfCourse);
            await _context.SaveChangesAsync();

            return Ok(golfCourse);
        }

        private bool CourseExists(Guid id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}
