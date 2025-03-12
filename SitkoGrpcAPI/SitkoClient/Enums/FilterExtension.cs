namespace SitkoClient.Enums
{
    public static class FilterExtension
    {
        public static string ToStringRus(this FilterActive filter)
        {
            switch (filter)
            {
                case FilterActive.SortAlphabetically:
                    return "В алфавитном порядке";
                case FilterActive.SortExecutionDate:
                    return "По дате закрытия";
                case FilterActive.SortCreationDate:
                    return "По дате создания";
                default: return filter.ToString();
            }
        }
    }
}