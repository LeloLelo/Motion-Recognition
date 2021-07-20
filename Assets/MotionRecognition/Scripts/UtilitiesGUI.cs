public class UtilitiesGUI
{
    const float underscoreSize = 5.5f;
    const float overscoreSize = 7.3f;
    public static string UpperSectionLimit(float inspectorSize = 0){
        string limit = "/";
        for (int i = 0; i < (int)(inspectorSize/overscoreSize); i++)
        {
            limit += '‾';
        }
        limit += "\\";
        return limit;
    }

    public static string LowerSectionLimit(float inspectorSize = 0){
        string limit = "\\";
        for (int i = 0; i < (int)(inspectorSize/underscoreSize); i++)
        {
            limit += '_';
        }
        limit += "/";
        return limit;
    }
}
