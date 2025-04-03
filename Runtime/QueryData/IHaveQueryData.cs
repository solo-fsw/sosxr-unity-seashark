using System.Collections.Generic;


namespace SOSXR.SeaShark.QueryData
{
    public interface IHaveQueryData
    {
        public string BaseURL { get; }

        public List<string> QueryStringVariables { get; }

        public string QueryStringURL { get; set; }


        public void ModifyVariables(string value)
        {
            if (QueryStringVariables.Contains(value))
            {
                QueryStringVariables.Remove(value);
            }
            else
            {
                QueryStringVariables.Add(value);
            }

            UpdateQuery();
        }


        public void UpdateQuery()
        {
            QueryStringURL = this.BuildQueryURL(BaseURL, QueryStringVariables.ToArray());
        }


        public void ClearQuery()
        {
            QueryStringVariables.Clear();
            QueryStringURL = string.Empty;
        }
    }
}