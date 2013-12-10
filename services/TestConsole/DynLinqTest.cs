using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expr = System.Linq.Expressions;
using System.Linq.Dynamic;

namespace TestConsole
{
    public class Entity
    {
        public string Name
        {
            get;
            set;
        }
    }

    public class DynLinqTest
    {
        public static void Run()
        {
            Entity p1 = new Entity() {Name = "iP1"};
            Entity p2 = new Entity() {Name = "iP2"};
            String.Compare("a", "b");
            Expr.Expression<Func<Entity, Entity, int>> parsedExpression =
                    (Expr.Expression<Func<Entity, Entity, int>>)DynamicExpression.ParseLambda(
                    new Expr.ParameterExpression[] { Expr.Expression.Parameter(typeof(Entity), "p1"), Expr.Expression.Parameter(typeof(Entity), "p2") },
                    typeof(int), "p1.Name > p2.Name");

            var f = parsedExpression.Compile();
            Console.WriteLine(f(p1, p2));
            Console.WriteLine(f(p2, p1));

            
            Console.WriteLine(String.Compare(null, null));
            Console.WriteLine(DateTime.Now > null);
            Console.ReadLine();
        }
    }
}
