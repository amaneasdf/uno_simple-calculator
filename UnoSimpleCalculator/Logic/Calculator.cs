using System.Globalization;

namespace UnoSimpleCalculator.Logic;

public record Calculator
{
    private string? Number { get; init; }
    private string? Operator { get; init; }
    private double? Result { get; init; }
    private double? Number1 { get; init; }
    private double? Number2 { get; init; }
    private bool IsNumber2Percentage { get; init; }
    private bool HasOperator => !string.IsNullOrEmpty(Operator);
    private bool HasNumber => !string.IsNullOrEmpty(Number);
    private bool HasNumber1 => Number1 != null;

    public string Output =>
        $"{(Result != null ? Result.Value.ToString(CultureInfo.InvariantCulture) : HasNumber ? Number : "0")}";
    public string Equation =>
        $"{Number1?.ToString(CultureInfo.InvariantCulture)} {Operator} {Number2?.ToString(CultureInfo.InvariantCulture)}{(IsNumber2Percentage ? "%" : string.Empty)}{(Result != null ? $" =" : string.Empty)}";

    public Calculator Input(string key)
    {
        var calculator = RestartOrClear(key);
        return key switch
        {
            "÷" or "×" or "+" or "−" => OperatorKey(calculator, key),
            "back" => BackKey(calculator),
            "." => DotKey(calculator),
            "0" when !calculator.HasNumber => calculator,
            "C" => new(),
            "=" => EqualsKey(calculator),
            "%" => PercentageKey(calculator),
            "±" => PlusMinusKey(calculator),

            _ => calculator with { Number = calculator.Number + key }
        };
    }

    private Calculator RestartOrClear(string key)
    {
        if (Result != null)
        {
            if (key == "÷" || key == "×" || key == "+" || key == "−")
            {
                return this with
                {
                    Number1 = Result,
                    Result = null,
                    Number2 = null,
                    Number = null,
                    Operator = key,
                    IsNumber2Percentage = false
                };
            }
            else
            {
                return new();
            }
        }

        return this;
    }

    private static Calculator BackKey(Calculator calculator)
    {
        if (calculator.HasNumber)
        {
            calculator = calculator with { Number = calculator.Number?[..^1] };
        }

        return calculator;
    }

    private static Calculator DotKey(Calculator calculator)
    {
        if (calculator.HasNumber)
        {
            if (calculator.Number?.Contains('.') == false)
            {
                calculator = calculator with { Number = calculator.Number + "." };
            }
        }
        else
        {
            calculator = calculator with { Number = "0." };
        }

        return calculator;
    }

    private static Calculator EqualsKey(Calculator calculator)
    {
        if (calculator.HasOperator && calculator.HasNumber1)
        {
            double? number2 = calculator.HasNumber ? GetNumber(calculator.Number) : 0.0;

            calculator = calculator with
            {
                Number2 = number2,
                Result = CalculateResult(
                    calculator.Operator,
                    calculator.Number1!.Value,
                    number2!.Value
                )
            };
        }

        return calculator;
    }

    private static Calculator PercentageKey(Calculator calculator)
    {
        if (calculator.HasOperator && calculator.HasNumber1)
        {
            double? number2 = calculator.HasNumber ? GetNumber(calculator.Number) : 0.0;
            double percentage = number2!.Value / 100 * calculator.Number1!.Value;

            calculator = calculator with
            {
                Number2 = number2,
                Result = CalculateResult(
                    calculator.Operator,
                    calculator.Number1!.Value,
                    percentage
                ),
                IsNumber2Percentage = true
            };
        }

        return calculator;
    }

    private static Calculator PlusMinusKey(Calculator calculator)
    {
        if (calculator.HasNumber)
        {
            calculator = calculator with
            {
                Number =
                    calculator.Number?.StartsWith("-") == true
                        ? calculator.Number?[1..]
                        : "-" + calculator.Number
            };
        }
        return calculator;
    }

    private static Calculator OperatorKey(Calculator calculator, string key)
    {
        if (calculator.HasNumber)
        {
            if (calculator.HasOperator && calculator.HasNumber1)
            {
                calculator = EqualsKey(calculator);

                calculator = calculator with
                {
                    Operator = key,
                    Number1 = calculator.Result,
                    Number = null,
                    Number2 = null,
                    Result = null
                };
            }
            else if (!calculator.HasOperator)
            {
                calculator = calculator with
                {
                    Operator = key,
                    Number1 = GetNumber(calculator.Number),
                    Number = null
                };
            }
        }
        else if (calculator.HasOperator && calculator.HasNumber1)
        {
            calculator = calculator with { Operator = key };
        }

        return calculator;
    }

    private static double? GetNumber(string? number) =>
        Convert.ToDouble(number, CultureInfo.InvariantCulture);

    /// <summary>
    /// Calculates the result of a mathematical operation based on the operator and two numbers.
    /// </summary>
    /// <param name="operator">The operator representing the mathematical operation.</param>
    /// <param name="firstNumber">The first number.</param>
    /// <param name="secondNumber">The second number.</param>
    /// <returns>The result of the mathematical operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the operator is not a valid mathematical operation.</exception>
    private static double CalculateResult(
        string? @operator,
        double firstNumber,
        double secondNumber
    ) =>
        @operator switch
        {
            "÷" => firstNumber / secondNumber,
            "×" => firstNumber * secondNumber,
            "+" => firstNumber + secondNumber,
            "-" => firstNumber - secondNumber,
            _ => throw new InvalidOperationException()
        };
}
