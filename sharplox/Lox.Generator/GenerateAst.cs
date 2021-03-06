using System.Globalization;
using System.Text;

namespace Lox.Generator;

public static class GenerateAst
{
  public static string DefineAst(string baseName, string[] types)
  {
    StringBuilder builder = new();

    builder.AppendLine("using Lox.Interpreter.Lexer;")
           .AppendLine()
           .AppendLine("namespace Lox.Interpreter.Ast;")
           .AppendLine()
           .AppendLine($"public abstract class {baseName}")
           .AppendLine("{");

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

    builder.AppendLine("  public abstract R Accept<R>(IVisitor<R> visitor);")
           .AppendLine("}");

    return builder.ToString();
  }

  private static void DefineVisitor(StringBuilder builder, string baseName, string[] types)
  {
    builder.AppendLine($"  public interface IVisitor<R>")
           .AppendLine("  {");

    foreach (string type in types)
    {
      string typeName = type.Split(':')[0].Trim();
      builder.AppendLine($"    R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
    }

    builder.AppendLine("  }");
  }

  private static void DefineNestedType(StringBuilder builder, string baseName, string className, string fieldList)
  {
    TextInfo text = new CultureInfo("en-US").TextInfo;

    // Class
    builder.AppendLine($"  public class {className} : {baseName}")
           .AppendLine("  {");

    // Constructor
    builder.AppendLine($"    public {className}({fieldList})")
           .AppendLine("    {");

    // Initialize fields
    string[] fields = fieldList.Split(", ");
    foreach (string field in fields)
    {
      string name = field.Split(' ')[1];
      builder.AppendLine($"      {text.ToTitleCase(name)} = {name};");
    }

    builder.AppendLine("    }");

    // Visitor
    builder.AppendLine()
           .AppendLine($"    public override R Accept<R>(IVisitor<R> visitor)")
           .AppendLine("    {")
           .AppendLine($"      return visitor.Visit{className}{baseName}(this);")
           .AppendLine("    }");

    // Fields
    builder.AppendLine();
    foreach (string field in fields)
    {
      string[] split = field.Split(' ');
      string type = split[0];
      string name = text.ToTitleCase(split[1]);
      builder.AppendLine($"    public {type} {name} {{ get; }}");
    }

    builder.AppendLine("  }");
  }
}
