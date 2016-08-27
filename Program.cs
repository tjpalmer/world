using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace World {

public class Program {

  public static void Main(string[] args) {
    Expression<Func<double, double>> function = x => Math.Pow(x, 2);
    Console.WriteLine(string.Join(" ",
      "Hello World!",
      $"{function}",
      $"{function.Body.GetType()}",
      $"{function.CanReduce}")
    );
    var call = (MethodCallExpression)function.Body;
    Console.WriteLine(
      $"In call: {call.Method} {string.Join(" ", call.Arguments)}"
    );
    var method = call.Method;
    var pow = ((Func<double, double, double>)Math.Pow).GetMethodInfo();
    var min = ((Func<double, double, double>)Math.Min).GetMethodInfo();
    Console.WriteLine($"Is min? {method == min}");
    Console.WriteLine($"Is pow? {method == pow}");
    var argsList = new List<Expression>(call.Arguments).ToArray();
    var arguments = call.Arguments;
    foreach (var argument in arguments) {
      Console.WriteLine($"{argument} {argument.GetType()}");
    }
    var visitor = new Visitor();
    var after = visitor.Visit(function);
    Console.WriteLine($"From {function} to {after}");
  }

}

class Visitor: ExpressionVisitor {
  protected override Expression VisitConstant(ConstantExpression node) {
    Console.WriteLine($"Found constant: {node.Type} {node.Value}");
    return Expression.Constant(3.0);
  }
}

}
