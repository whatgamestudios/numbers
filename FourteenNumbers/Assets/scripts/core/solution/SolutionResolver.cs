// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using System.Collections;

namespace FourteenNumbers
{

    public class SolutionResolver
    {
        public static (string, bool, string, bool, string, bool) Resolve(string combinedSolution) 
        {
            string sol1 = "";
            bool complete1 = false;
            string sol2 = "";
            bool complete2 = false;
            string sol3 = "";
            bool complete3 = false;

            if (combinedSolution.Length != 0) {
                int indexOfEquals = combinedSolution.IndexOf('=');
                if (indexOfEquals == -1) 
                {
                    sol1 = combinedSolution;
                }
                else 
                {
                    complete1 = true;
                    sol1 = combinedSolution.Substring(0, indexOfEquals);

                    combinedSolution = combinedSolution.Substring(indexOfEquals+1);
                    indexOfEquals = combinedSolution.IndexOf('=');
                    if (indexOfEquals == -1) 
                    {
                        sol2 = combinedSolution;
                    }
                    else 
                    {
                        complete2 = true;
                        sol2 = combinedSolution.Substring(0, indexOfEquals);

                        combinedSolution = combinedSolution.Substring(indexOfEquals+1);
                        indexOfEquals = combinedSolution.IndexOf('=');
                        if (indexOfEquals == -1) 
                        {
                            sol3 = combinedSolution;
                        }
                        else 
                        {
                            complete3 = true;
                            sol3 = combinedSolution.Substring(0, indexOfEquals);
                        }
                    }
                }
            }
            return (sol1, complete1, sol2, complete2, sol3, complete3);
        } 

        public static (string, string, string) ResolveComplete(string combinedSolution) 
        {
            (string sol1, bool complete1, string sol2, bool complete2, string sol3, bool complete3) = Resolve(combinedSolution);

            sol1 = complete1 ? sol1 : "";
            sol2 = complete1 && complete2 ? sol2 : "";
            sol3 = complete1 && complete2 && complete3 ? sol3 : "";
            return (sol1, sol2, sol3);
        }         

        public static string ResolveInProgress(string combinedSolution) 
        {
            (string sol1, bool complete1, string sol2, bool complete2, string sol3, bool complete3) = Resolve(combinedSolution);
            if (!complete1) 
            {
                return sol1;
            }
            if (!complete2) 
            {
                return sol2;
            }
            return sol3;
        }         
    }    
}