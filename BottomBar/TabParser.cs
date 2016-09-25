using System;
using System.Collections.Generic;
using System.Xml;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;

namespace BottomBarSharp {
    public class TabParser {
        private readonly Context context;
        private readonly BottomBarTabConfig defaultTabConfig;
        private readonly XmlReader parser;

        public List<BottomBarTab> Tabs { get; }
        private BottomBarTab workingTab;

        public TabParser(Context context,BottomBarTabConfig defaultTabConfig,int tabsXmlResId) {
            this.context = context;
            this.defaultTabConfig = defaultTabConfig;

            parser = context.Resources.GetXml(tabsXmlResId);
            Tabs = new List<BottomBarTab>();

            parse();
        }

        private void parse() {
            using(parser) {
                while(parser.Read()) {
                    if(parser.NodeType == XmlNodeType.Element) {
                        if(parser.Name == "tab") {
                            parseNewTab();
                        }
                    } else if(parser.NodeType == XmlNodeType.EndElement) {
                        if(parser.Name == "tab" && workingTab != null) {
                            Tabs.Add(workingTab);
                            workingTab = null;
                        }
                    }
                }
            }
        }

        private void parseNewTab() {
            if(parser.HasAttributes == false) {
                return;
            }

            if(workingTab == null) {
                workingTab = tabWithDefaults();
            }

            workingTab.IndexInContainer = Tabs.Count;

            for(int i = 0;i < parser.AttributeCount;i++) {
                parser.MoveToAttribute(i);

                if(parser.HasValue == false) {
                    continue;
                }

                switch(parser.Name) {
                    case "id": {
                            int? id = getResourceId();

                            if(id != null) {
                                workingTab.Id = id.Value;
                            }
                            break;
                        }
                    case "icon": {
                            int? iconResId = getResourceId();

                            if(iconResId != null) {
                                workingTab.IconResId = iconResId.Value;
                            }
                            break;
                        }
                    case "title": {
                            workingTab.Title = getTitleValue();
                            break;
                        }
                    case "inActiveColor": {
                            int? inActiveColor = getColorValue();

                            if(inActiveColor != null) {
                                workingTab.InActiveAlpha = inActiveColor.Value;
                            }
                            break;
                        }
                    case "activeColor": {
                            int? activeColor = getColorValue();

                            if(activeColor != null) {
                                workingTab.ActiveColor = activeColor.Value;
                            }
                            break;
                        }
                    case "barColorWhenSelected": {
                            int? barColorWhenSelected = getColorValue();

                            if(barColorWhenSelected != null) {
                                workingTab.BarColorWhenSelected = barColorWhenSelected.Value;
                            }
                            break;
                        }
                    case "badgeBackgroundColor": {
                            int? badgeBackgroundColor = getColorValue();

                            if(badgeBackgroundColor != null) {
                                workingTab.BadgeBackgroundColor = badgeBackgroundColor.Value;
                            }
                            break;
                        }
                }
            }
            parser.MoveToElement();

        }

        private BottomBarTab tabWithDefaults() {
            var tab = new BottomBarTab(context);
            tab.SetConfig(defaultTabConfig);

            return tab;
        }

        private string getTitleValue() {
            int? titleResource = getResourceId();
            if(titleResource != null) {
                return context.GetString(titleResource.Value);
            }

            return parser.Value;
        }

        private int? getColorValue() {
            int? colorResouce = getResourceId();

            if(colorResouce != null) {
                return ContextCompat.GetColor(context,colorResouce.Value);
            }

            try {
                return Color.ParseColor(parser.Value);
            } catch(Exception) {
                return null;
            }
        }

        private int? getResourceId() {
            int? resourceId;
            try {
                string type = parser.Value.TrimStart('@','+').Split('/')[0];
                string name = parser.Value.Split('/')[1];
                resourceId = context.Resources.GetIdentifier(name,type,context.PackageName);
            } catch(Exception) {
                resourceId = null;
            }
            return resourceId;
        }
    }
}