// Copyright (c) 2013, 2015 Robert Rouhani <robert.rouhani@gmail.com> and other contributors (see CONTRIBUTORS file).
// Licensed under the MIT License - https://raw.github.com/Robmaister/SharpNav/master/LICENSE

using System;
using System.Collections.Generic;
using System.IO;



namespace Simulate
{
    /// <summary>
    /// Parses a model in .obj format.
    /// </summary>
    public class Read
    {
        //public IList<AgentClass> _agents = new List<AgentClass>();
        //private static readonly char[] lineSplitChars = { ' ' };
        //public Read(string path)
        //{
        //    using (StreamReader reader = new StreamReader(path))
        //    {
        //        string file = reader.ReadToEnd();
        //        int Nov = 0;
        //        foreach (string frame in file.Split('\n'))
        //        {
        //            //trim any extras
        //            string tl = frame;
        //            string[] line = tl.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries);
        //            if (line == null || line.Length == 0)
        //                continue;
        //            _agents.Clear(); 
                    
        //            for(int i=0;i<line.Length-1;i++)
        //            {
        //                _agents.Add(new AgentClass(_agents.Count, new Vector2(float.Parse(line[i]), float.Parse(line[i+1])), new Vector2(0, 0), AgentStates.Enter));
        //            }
                    
        //        }
        //    }
        //}
    }
}
