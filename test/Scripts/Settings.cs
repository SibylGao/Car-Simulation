using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Settings
{
 

    public Settings()
    {

    }

    public static Settings RVODefault
    {
        get
        {
            //TODO rename this property to something more descriptive.
            var settings = new Settings();

            settings.deltaT = 0.25f;
            settings.neighborDist = 3f;
            settings.maxNeighbors = 6;
            settings.timeHorizon = 6f;
            settings.timeHorizonObst = 6f;
            settings.radius = 0.2f;
            settings.maxSpeed = 1;
            return settings;
        }
    }

    //一些RVO参数
    public float deltaT { get; set; }
    public float neighborDist { get; set; }
    public int maxNeighbors { get; set; }
    public float timeHorizon { get; set; }
    public float timeHorizonObst { get; set; }
    public float radius { get; set; }
    public float maxSpeed { get; set; }


}

