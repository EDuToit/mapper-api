
(SELECT l."UserID", MIN(ST_AsGeoJson(ST_geomFromWKB(l."PointRaw"))) AS PlayerLocation From public."LiveLocation" l Where ST_Contains(	
	(SELECT ST_Buffer(
					(SELECT (ST_Centroid(ST_Union(ST_GeomFromWKB(ie."PointRaw")))) FROM public."Elements" ie
					where ie."CourseId" = '6ee6896e-33ef-4bd4-8aa3-0b0c67129615'), --Point

					(SELECT max(ST_DistanceSphere(
							ST_geomFromWKB(e."PolygonRaw"), 
							(SELECT (ST_Centroid(ST_Union(ST_GeomFromWKB(ie."PointRaw")))) FROM public."Elements" ie
							where ie."CourseId" = '6ee6896e-33ef-4bd4-8aa3-0b0c67129615')
						))
						FROM public."Elements" e 
						WHERE e."CourseId" = '6ee6896e-33ef-4bd4-8aa3-0b0c67129615')/1000, -- Radius
						'quad_segs=8' --Form
		)),
		ST_GeomFromWKB(l."PointRaw")
	) AND (EXTRACT(MINUTE FROM  (now() - l."CreatedAt")) < 10) -- Limits the result to only the last 10 minutes
 	GROUP BY l."UserID"
 	ORDER BY MIN(l."CreatedAt")
)

-- '6ee6896e-33ef-4bd4-8aa3-0b0c67129615' THIS IS A COURSE ID