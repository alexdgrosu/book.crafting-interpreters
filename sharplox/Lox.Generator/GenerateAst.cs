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

    for (int i = 0; i < types.Length; i++)
    {
      var split = types[i].Split(':');
      string className = split[0].Trim();
      string fields = split[1].Trim();

      DefineNestedType(builder, baseName, className, fields);

      if (!IsLast(i))
      {
        builder.AppendLine();
      }
    }

    builder.AppendLine("}");

    return builder.ToString();

    bool IsLast(int index) => index == types.Length - 1;
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

    // Fields
    builder.AppendLine();
    foreach (string field in fields)
    {
      builder.AppendLine($"    private readonly {field};");
    }

    builder.AppendLine("  }");
  }
}
