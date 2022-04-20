using System.Text;

namespace Lox.Generator;

public static class GenerateAst
{
  public static string DefineAst(string baseName, string[] types)
  {
    StringBuilder builder = new();

    builder.AppendLine("using Lox.Interpreter.Lexer;");
    builder.AppendLine();
    builder.AppendLine("namespace Lox.Interpreter.Ast;");
    builder.AppendLine();
    builder.AppendLine($"public abstract class {baseName}");
    builder.AppendLine("{");

    DefineVisitor(builder, baseName, types);
    builder.AppendLine();

    for (int i = 0; i < types.Length; i++)
    {
      var split = types[i].Split(':');
      string className = split[0].Trim();
      string fields = split[1].Trim();

      DefineNestedType(builder, baseName, className, fields);
      builder.AppendLine();
    }
    builder.AppendLine("  public abstract R Accept<R>(IVisitor<R> visitor);");
    builder.AppendLine("}");

    return builder.ToString();

  }

  private static void DefineVisitor(StringBuilder builder, string baseName, string[] types)
  {
    builder.AppendLine($"  public interface IVisitor<R>");
    builder.AppendLine("  {");

    foreach (string type in types)
    {
      string typeName = type.Split(':')[0].Trim();
      builder.AppendLine($"    R visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
    }
    builder.AppendLine("  }");
  }

  private static void DefineNestedType(StringBuilder builder, string baseName, string className, string fieldList)
  {
    // Class
    builder.AppendLine($"  public class {className} : {baseName}");
    builder.AppendLine("  {");

    // Constructor
    builder.AppendLine($"    public {className}({fieldList})");
    builder.AppendLine("    {");

    // Initialize fields
    string[] fields = fieldList.Split(", ");
    foreach (string field in fields)
    {
      string name = field.Split(' ')[1];
      builder.AppendLine($"      this.{name} = {name};");
    }

    builder.AppendLine("    }");

    // Visitor
    builder.AppendLine();
    builder.AppendLine($"    public override R Accept<R>(IVisitor<R> visitor)");
    builder.AppendLine("    {");
    builder.AppendLine($"      return visitor.visit{className}{baseName}(this);");
    builder.AppendLine("    }");

    // Fields
    builder.AppendLine();
    foreach (string field in fields)
    {
      builder.AppendLine($"    private readonly {field};");
    }

    builder.AppendLine("  }");
  }
}
