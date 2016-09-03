using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.ObjectModel;
using static System.Math;

namespace World {

public class Program {

  public static void Main(string[] args) {
    var function = Expr(x => Pow(x, 2));
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
    Console.WriteLine($"Is min? {call.Method == Method(Min)}");
    Console.WriteLine($"Is pow? {call.Method == Method(Pow)}");
    var argsList = new List<Expression>(call.Arguments).ToArray();
    var arguments = call.Arguments;
    foreach (var argument in arguments) {
      Console.WriteLine($"{argument} {argument.GetType()}");
    }
    var visitor = new Visitor();
    var after = visitor.Visit(function);
    Console.WriteLine($"From {function} to {after}");
    Console.WriteLine(visitor.Visit(Expr(x => x + 1)));
  }

  static Expression<Func<double, double>> Expr(
    Expression<Func<double, double>> expression
  ) {
    return expression;
  }

  static MethodInfo Method(Func<double, double, double> function) {
    return function.GetMethodInfo();
  }

}

class Function: Expression {

  public Function(
    Type type,
    Expression body,
    ReadOnlyCollection<Expression> parameters
  ) {
    this.Body = body;
    this.Parameters = parameters;
    this.type = type;
  }

  public readonly Expression Body;

  public sealed override ExpressionType NodeType {
    get {return ExpressionType.Lambda;}
  }

  public readonly ReadOnlyCollection<Expression> Parameters;

  public override string ToString() {
    return string.Join(" ",
      Parameters.Count == 1 ?
        Parameters[0].ToString() :
        $"({string.Join(", ", Parameters)})",
      "=>",
      Body
    );
  }

  public sealed override Type Type {
    get {return type;}
  }

  private readonly Type type;

}

class Variable: Expression {

  public Variable(Type type, string name) {
    this.type = type;
    this.Name = name;
  }

  public sealed override ExpressionType NodeType {
    get {return ExpressionType.Parameter;}
  }

  public readonly string Name;

  public override string ToString() {
    // TODO Change this back to just the name.
    // TODO It's only extra so I can see that I've got it in place.
    return $"{Name}!?!";
  }

  public sealed override Type Type {
    get {return type;}
  }

  private Type type;

}

class Visitor: ExpressionVisitor {
  protected override Expression VisitConstant(ConstantExpression node) {
    Console.WriteLine($"Found constant: {node.Type} {node.NodeType} {node.Value}");
    return Expression.Constant(3.0);
  }

  protected override Expression VisitLambda<T>(Expression<T> node) {
    return new Function(
      // The same type should apply if we just replace old parameters with our
      // metadata-carrying equivalents.
      node.Type,
      Visit(node.Body),
      new List<Expression>(
        from parameter in node.Parameters select Visit(parameter)
      ).AsReadOnly()
    );
  }

  protected override Expression VisitParameter(ParameterExpression node) {
    Console.WriteLine($"Found parameter: {node.Type} {node.NodeType} {node.Name}");
    return new Variable(node.Type, node.Name);
  }
}

}
