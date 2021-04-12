﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Ganss.XSS;

namespace Core.Shared
{
    public class FormateadorService
    {
        private readonly HtmlEncoder encoder;
        private readonly static HtmlSanitizer sanitizer =
            new HtmlSanitizer("a span".Split(" "),
            allowedAttributes: "href target class r-id".Split(" "));

        public FormateadorService(HtmlEncoder encoder)
        {
            this.encoder = encoder;
        }

        public string Parsear(string contenido)
        {
            var tags = new List<string>();

            var x = contenido
                    .Split("\n")
                    .Select(t =>
                    {
                        t = encoder.Encode(t);
                        var esLink = false;
                        var esTag = false;

                        //Links
                        t = Regex.Replace(t, @"&gt;(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b(\S*)", m =>
                        {
                            var link = m.Value.Replace("&gt;", "");                            
                            esLink = true;

                            //VERSION SEGURA para que solo acceda a link con SSL
                            //link = Regex.Replace(link, @"(http(s)?:\/\/)", "");
                            //return $@"<a href=""//{link}"" target=""_blank"">&gt;{link}</a>";

                            return $@"<a href=""{link}"" target=""_blank"">&gt;{link}</a>";
                        });

                        if (esLink) return t;

                        if (t.StartsWith("&gt;&gt;"))
                        {
                            var id = t
                            .Replace("&gt;&gt;", "")
                            .Replace("&#xD;", "")
                            .Trim();
                            return $"<a href=\"#{id}\" data-quote=\"{id}\">&gt;&gt;{id}</a>";
                        }

                        //Respuestas
                        t = Regex.Replace(t, @"&gt;&gt;([A-Z0-9]{8})", m => {
                            esTag = true;
                            if (tags.Contains(m.Value)) return "";
                            esTag = true;
                            tags.Add(m.Value);
                            var id = m.Groups[1].Value;
                            return $"<a href=\"#{id}\" class=\"restag\" r-id=\" {id}\">&gt;&gt;{id}</a>";
                        });


                        //Texto verde
                        t = Regex.Replace(t.Replace("&#xA;", "\n"), @"&gt;(?!https?).+(?:$|\n)", m =>
                        {
                            if (esLink || esTag) return m.Value;
                            var text = m.Value;
                            return $@"<span class=""greentext"">{text}</span>";
                        });

                        return t;

                    });

            var ret = string.Join("\n", x);
            return ret;

            //VERSION SEGURA PROBARLO A FUTURO
            //var sanitezed = sanitizer.Sanitize(ret);
            //return sanitezed;
        }

        public string[] GetIdsTageadas(string contenido)
        {
            return Regex.Matches(contenido, @">>([A-Z0-9]{8})")
                .Select(m => {
                    return m.Groups[1].Value;
                }).ToArray();
        }
    }
}