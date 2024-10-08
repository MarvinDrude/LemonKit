namespace LemonKit.SimpleDemo.Validation;

public static class StringValidations {

    public static bool AreHexColors(this List<string> strs) {

        return !strs.Any(x => !x.IsHexColor());

    }

    public static bool IsHexColor(this string str) {

        if(str[0] != '#') {
            return false;
        }

        if(str.Length is not 4 and not 7) {
            return false;
        }

        return !str[1..].Any(x => !Uri.IsHexDigit(x));

    }

}
