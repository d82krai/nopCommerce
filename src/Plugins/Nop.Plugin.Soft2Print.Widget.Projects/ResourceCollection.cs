using System.Collections.Generic;

namespace Nop.Plugin.Soft2Print.Widget.Projects
{
    public class ResourceCollection
    {
        public class Key
        {
            private const string prefix = "Plugins.Soft2Print.Projects.Fields.";

            public class General
            {
                private const string prefix = Key.prefix + "General.";

                public class Action
                {
                    private const string prefix = General.prefix + "Action.";

                    public const string Rename = prefix + "Rename";
                    public const string Copy = prefix + "Copy";
                    public const string Delete = prefix + "Delete";
                }
                public class OpenEditor
                {
                    private const string prefix = General.prefix + "OpenEditor.";

                    public const string Edit = prefix + "Edit";
                    public const string View = prefix + "View";
                }
                public class Details
                {
                    private const string prefix = General.prefix + "Details.";

                    public const string Created = prefix + "Created";
                    public const string LastChanged = prefix + "LastChanged";
                    public const string Theme = prefix + "Theme";
                    public const string InheritedBy = prefix + "InheritedBy";
                    public const string Product = prefix + "Product";
                }
            }

            public class HeaderLink
            {
                private const string prefix = Key.prefix + "HeaderLink.";

                public const string Label = prefix + "Label";
            }

            public class AccountLink
            {
                private const string prefix = Key.prefix + "AccountLink.";

                public const string Label = prefix + "Label";
            }

            public class ProductDetails
            {
                private const string prefix = Key.prefix + "ProductDetails.";

                public const string Header = prefix + "Header";
            }

            public class DedicatedProjectList
            {
                private const string prefix = Key.prefix + "DedicatedProjectList.";

                public const string Header = prefix + "Header";
                public const string NoProjects = prefix + "NoProjects";
            }


            public class Admin
            {
                private const string prefix = Key.prefix + "Admin.";

                public class General
                {
                    private const string prefix = Admin.prefix + "General.";

                    public class Action
                    {
                        private const string prefix = General.prefix + "Action.";

                        public const string Header = prefix + "Header";

                        public const string ShowCopy = prefix + "ShowCopy";
                        public const string ShowRename = prefix + "ShowRename";
                        public const string ShowDelete = prefix + "ShowDelete";
                    }
                    public class Details
                    {
                        private const string prefix = General.prefix + "Details.";

                        public const string Header = prefix + "Header";

                        public const string ShowCreated = prefix + "ShowCreated";
                        public const string ShowLastChanged = prefix + "ShowLastChanged";
                        public const string ShowProduct = prefix + "ShowProduct";
                        public const string ShowPreview = prefix + "ShowPreview";
                        public const string ShowTheme = prefix + "ShowTheme";
                        public const string GetWarningIfS2PThemeIsShown = prefix + "GetWarningIfS2PThemeIsShown";

                        public const string ShowInheritedBy = prefix + "ShowInheritedBy";
                        public const string DefaultProjectName = prefix + "DefaultProjectName";
                    }
                }

                public class ProductDetails
                {
                    private const string prefix = Admin.prefix + "ProductDetails.";

                    public const string Header = prefix + "Header";

                    public const string Show = prefix + "Show";
                    public const string HideIfGuest = prefix + "HideIfGuest";
                    public const string ViewMode = prefix + "ViewMode";
                    public const string WidgetZone = prefix + "WidgetZone";
                }

                public class HeaderLink
                {
                    private const string prefix = Admin.prefix + "HeaderLink.";

                    public const string Header = prefix + "Header";

                    public const string Show = prefix + "Show";
                    public const string HideIfGuest = prefix + "HideIfGuest";
                    public const string WidgetZone = prefix + "WidgetZone";
                }

                public class AccountLink
                {
                    private const string prefix = Admin.prefix + "AccountLink.";

                    public const string Header = prefix + "Header";

                    public const string Show = prefix + "Show";
                    public const string WidgetZone = prefix + "WidgetZone";
                }
            }



        }

        public static Dictionary<string, string> GetDefaultValues()
        {
            var defaultTexts = new Dictionary<string, string>();

            #region General
            // Actions
            defaultTexts.Add(Key.General.Action.Rename, "Rename");
            defaultTexts.Add(Key.General.Action.Copy, "Copy");
            defaultTexts.Add(Key.General.Action.Delete, "Delete");

            // OpenEditor
            defaultTexts.Add(Key.General.OpenEditor.Edit, "Edit");
            defaultTexts.Add(Key.General.OpenEditor.View, "View");

            // OpenEditor
            defaultTexts.Add(Key.General.Details.Created, "Created:");
            defaultTexts.Add(Key.General.Details.LastChanged, "Last changed:");
            defaultTexts.Add(Key.General.Details.Theme, "Theme:");
            defaultTexts.Add(Key.General.Details.InheritedBy, "History:");
            defaultTexts.Add(Key.General.Details.Product, "Product:");
            #endregion

            // ProductDetails
            defaultTexts.Add(Key.ProductDetails.Header, "Projects");

            // HeaderLink
            defaultTexts.Add(Key.HeaderLink.Label, "Projects");

            // AccountLink
            defaultTexts.Add(Key.AccountLink.Label, "My Projects");

            // DedicatedProjectList
            defaultTexts.Add(Key.DedicatedProjectList.Header, "My Projects");
            defaultTexts.Add(Key.DedicatedProjectList.NoProjects, "There are no projects");

            #region Admin
            // General //Action
            defaultTexts.Add(Key.Admin.General.Action.Header, "<b>General project actions</b>");
            defaultTexts.Add(Key.Admin.General.Action.ShowCopy, "Show copy");
            defaultTexts.Add(Key.Admin.General.Action.ShowDelete, "Show delete");
            defaultTexts.Add(Key.Admin.General.Action.ShowRename, "Show rename");

            // General //Details
            defaultTexts.Add(Key.Admin.General.Details.Header, "<b>General project details</b>");
            defaultTexts.Add(Key.Admin.General.Details.DefaultProjectName, "Default project name");
            defaultTexts.Add(Key.Admin.General.Details.ShowCreated, "Show created");
            defaultTexts.Add(Key.Admin.General.Details.ShowInheritedBy, "Show inherited by");
            defaultTexts.Add(Key.Admin.General.Details.ShowLastChanged, "Show last changed");
            defaultTexts.Add(Key.Admin.General.Details.ShowPreview, "Show preview");
            defaultTexts.Add(Key.Admin.General.Details.ShowProduct, "Show product");
            defaultTexts.Add(Key.Admin.General.Details.ShowTheme, "Show theme");
            defaultTexts.Add(Key.Admin.General.Details.GetWarningIfS2PThemeIsShown, "Get a warning if a Soft2Print theme name is show");

            defaultTexts.Add(Key.Admin.ProductDetails.Header, "<b>Project list on the product details page</b>");
            defaultTexts.Add(Key.Admin.ProductDetails.Show, "Show");
            defaultTexts.Add(Key.Admin.ProductDetails.HideIfGuest, "Hide if the customer is a guest");
            defaultTexts.Add(Key.Admin.ProductDetails.ViewMode, "View mode");
            defaultTexts.Add(Key.Admin.ProductDetails.WidgetZone, "Widged zone");

            defaultTexts.Add(Key.Admin.HeaderLink.Header, "<b>Header link</b>");
            defaultTexts.Add(Key.Admin.HeaderLink.Show, "Show");
            defaultTexts.Add(Key.Admin.HeaderLink.HideIfGuest, "Hide if the customer is a guest");
            defaultTexts.Add(Key.Admin.HeaderLink.WidgetZone, "Widged zone");

            defaultTexts.Add(Key.Admin.AccountLink.Header, "<b>Account link</b>");
            defaultTexts.Add(Key.Admin.AccountLink.Show, "Show");
            defaultTexts.Add(Key.Admin.AccountLink.WidgetZone, "Widged zone");
            #endregion

            return defaultTexts;
        }
    }
}
