using System;
using System.Collections.Generic;

namespace Mapper_Api.Services
{
    public class WeatherResult
    {
        public CoordEntity coord { get; set; }
        public List<WeatherEntity> Weather { get; set; }
        public string Base { get; set; }
        public MainEntity main { get; set; }
        public string Visibility { get; set; }
        public WindEntity Wind { get; set; }
        public CloudEntity Clouds { get; set; }
        public string dt { get; set; }
        public SysEntity sys { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string cod { get; set; }


        public class WeatherEntity
        {
            public string id { get; set; }
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
        }

        public class CoordEntity
        {
            public double lon { get; set; }
            public double lat { get; set; }

        }

        public class MainEntity
        {
            public double temp { get; set; }
            public string pressure { get; set; }
            public double humidity { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }
        }

        public class WindEntity
        {
            public double Speed { get; set; }
            public double Deg { get; set; }
        }

        public class CloudEntity
        {
            public int All { get; set; }
        }

        public class SysEntity
        {
            public int Type { get; set; }
            public int Id { get; set; }
            public double Message { get; set; }
            public String Country { get; set; }
            public int Sunrise { get; set; }
            public int Sunset { get; set; }
        }
    }
}
