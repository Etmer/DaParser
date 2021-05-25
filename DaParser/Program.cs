using EventScript;
using System;
using System.Collections.Generic;

namespace DaScript
{
    class Program
    {
        static string s =
@"              
            [Start]
                {Text = 'Test one'}
                    {Choice = 'Test choice one' => 'End'}
                    {Choice = 'Test choice two' => 'End'}
                    {Choice = 'Test choice three' => 'End'};
            end

            [End]
                 {Text = 'Test one' => 'Start'}; 
            end
        ";

    }
    
}
