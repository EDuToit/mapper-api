SELECT cs.* FROM public."Courses"  cs
	LEFT JOIN (
		SELECT *, ST_DistanceSphere(
 			ST_geomFromWkb((el."PolygonRaw")),
 			ST_geomFromGeoJson('{"type":"Point","coordinates":[31.576763, -24.986592]}') 
		   )FROM public."Elements" AS el 
		) AS e
	ON e."CourseId" = cs."CourseId"
	GROUP BY cs."CourseId"
	ORDER BY MIN(e."st_distancesphere")
	LIMIT 2
	
