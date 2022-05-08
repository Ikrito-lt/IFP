﻿using System.Collections.Generic;

namespace IFP.Modules.PiguIntegration.Models
{
    internal class xPiguProduct
    {
        public string categoryID { set; get; }
        public string categoryName { set; get; }
        public string title { set; get; }
        public string titleRU { set; get; }
        public string titleLV { set; get; }
        public string titleEE { set; get; }
        public string longDesc { set; get; }
        public string longDescRU { set; get; }
        public string longDescLV { set; get; }
        public string longDescEE { set; get; }
        public string YTLink { set; get; }

        public List<xPiguProductProperty> properties = new List<xPiguProductProperty>();
        public List<xPiguProductColor> colors = new List<xPiguProductColor>();

        public string GetPropertiesXml()
        {
            string s = "";
            foreach (xPiguProductProperty p in properties)
            {
                s += $"{p.GetXml()}\n";
            }
            return s;
        }

        public string GetColorsXml()
        {
            string s = "";
            foreach (xPiguProductColor c in colors)
            {
                s += $"{c.GetXml()}\n";
            }
            return s;
        }

        public string GetXml()
        {
            string s = $@" 
            <product>
                <category-id>{categoryID}</category-id>
                <category-name><![CDATA[{categoryName}]]></category-name>
                <title><![CDATA[{title}]]></title>
                <title-ru><![CDATA[{titleRU}]]></title-ru>
                <title-lv><![CDATA[{titleLV}]]></title-lv>
                <title-ee><![CDATA[{titleEE}]]></title-ee>
                <long-description><![CDATA[{longDesc}]]></long-description>
                <long-description-ru><![CDATA[{longDescRU}]]></long-description-ru>
                <long-description-lv><![CDATA[{longDescLV}]]></long-description-lv>
                <long-description-ee><![CDATA[{longDescEE}]]></long-description-ee>
                <video-youtube>{YTLink}</video-youtube>
                <properties>{GetPropertiesXml()}</properties>
                <colours>{GetColorsXml()}</colours>
            </product>";

            return s;
        }
    }
}
