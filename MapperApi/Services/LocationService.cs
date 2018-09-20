/***
 * Filename: LocationService.cs
 * Author : nielpretorius
 * Class   : LocationService
 *
 *      Process user location.
 ***/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Mapper_Api.Context;
using Mapper_Api.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;

namespace Mapper_Api.Services
{
    public class LocationService {
        private readonly CourseDb _db;

        public LocationService(CourseDb courseDb)
        {
            _db = courseDb;
        }

        public async Task<IEnumerable<Course>> sortCourseByPosition(Double? lat, Double? lon, int limit){
            string query = @"SELECT cs.* FROM public.""Courses"" cs LEFT JOIN 
            (SELECT *, ST_DistanceSphere(ST_geomFromWkb((el.""PolygonRaw"")), 
             ST_geomFromGeoJson('{{""type"":""Point"",""coordinates"":[ Param1 , Param2 ]}}'))
             FROM public.""Elements"" AS el) AS e ON e.""CourseId"" = cs.""CourseId""   
              GROUP BY cs.  ""CourseId"" ORDER BY MIN(e.""st_distancesphere"") LIMIT Param3";

            query = query.Replace("Param1", lat.ToString());
            query = query.Replace("Param2", lon.ToString());
            query = query.Replace("Param3", limit.ToString());
            
            List<Course> list = await _db.Courses.FromSql(query).ToListAsync();
            return list;
        }

        public async Task<IEnumerable<LiveLocation>> getRecentPlayerLocation(String courseID){
            string query = @"(SELECT l.""UserID"", MIN(ST_AsGeoJson(ST_geomFromWKB(l.""PointRaw""))) AS 
            PlayerLocation From public.""LiveLocation"" l WHERE 
            ST_Contains(	
                    (SELECT 
                        ST_Buffer(
                            (SELECT (ST_Centroid(ST_Union(ST_GeomFromWKB(ie.""PointRaw"")))) FROM
                                public.""Elements"" ie WHERE ie.""CourseId"" = '@Param1'),
                            (SELECT max(ST_DistanceSphere(
                                ST_geomFromWKB(e.""PolygonRaw""), 
                                (SELECT (ST_Centroid(ST_Union(ST_GeomFromWKB(ie.""PointRaw"")))) FROM 
                                public.""Elements"" ie
                                WHERE ie.""CourseId"" = '@Param1')
                            ))
                            FROM public.""Elements"" e 
                            WHERE e.""CourseId"" = '@Param1')/1000,
                        'quad_segs=8'
                        )),
                    ST_GeomFromWKB(l.""PointRaw"")
            ) AND (EXTRACT(MINUTE FROM  (now() - l.""CreatedAt"")) < 10)
            GROUP BY l.""UserID""
            ORDER BY MIN(l.""CreatedAt"")
            )";
            query = query.Replace("@Param" , courseID);

            List<LiveLocation> list = await _db.LiveLocation.FromSql(query).ToListAsync();
            return list;
        }
        
    }
}