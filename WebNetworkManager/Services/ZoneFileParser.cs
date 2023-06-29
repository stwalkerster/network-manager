using System.Text.RegularExpressions;

namespace DnsWebApp.Services;

using System;
using System.Collections.Generic;
using System.IO;
using Models;
using Models.Database;

class ZoneFileParser {

    private string currentOrigin = null;
    private uint currentTtl = 300;
    private uint originalTtl = uint.MaxValue;
    private readonly Stream zoneFileStream;
    private readonly string zoneName;

    public ZoneFileParser(Stream zoneFileStream, string zoneName)
    {
        this.zoneFileStream = zoneFileStream;
        this.zoneName = zoneName;
    }
    
    public List<Record> Parse()
    {
        var sr = new StreamReader(this.zoneFileStream);
    
        var initialMatch = new Regex(
            @"^(?<name>[^ \t]+)?\s+(?:(?<ttl1>[0-9]+)\s+)?(?:(?<class>IN|CS|CH|HS)\s+)?(?:(?<ttl2>[0-9]+)\s+)?(?<type>[A-Z]+)\s+(?<rdata>.*)$");
    
        var lastName = "@";

        var parsedRecords = new List<Record>();
        
        while (!sr.EndOfStream)
        {
            var data = sr.ReadLine();
    
            if (this.ParseDirective(data, "$ORIGIN", out var newOrigin))
            {
                this.currentOrigin = newOrigin;
                continue;
            }
    
            if (this.ParseDirective(data, "$TTL", out var newTtl))
            {
                this.currentTtl = uint.Parse(newTtl);
                if (this.originalTtl == uint.MaxValue)
                {
                    this.originalTtl = this.currentTtl;
                }
                
                continue;
            }
            
            Console.WriteLine(">{0}", data);
    
            var m = initialMatch.Match(data);

            if (!m.Success)
            {
                continue;
            }

            var name = m.Groups["name"].Success ? m.Groups["name"].Value : lastName;
            lastName = name;

            var rrClass = m.Groups["class"].Success ? m.Groups["class"].Value : "IN";
            uint? ttl = m.Groups["ttl1"].Success
                ? uint.Parse(m.Groups["ttl1"].Value)
                : (m.Groups["ttl2"].Success ? uint.Parse(m.Groups["ttl2"].Value) : (this.currentTtl != this.originalTtl ? this.currentTtl : null));

            var type = m.Groups["type"].Value;
            var rdata = m.Groups["rdata"].Value;


            var inQuote = false;
            var inBracket = false;

            var combinedRdata = "";
            var parsedRdata = "";

            do
            {
                var inComment = false;
                
                for (int i = 0; i < rdata.Length; i++)
                {
                    var c = rdata[i];
                    
                    if (c == '"')
                    {
                        inQuote = !inQuote;
                    }

                    if (!inQuote && !inComment)
                    {
                        switch (c)
                        {
                            case '(':
                                inBracket = true;
                                break;
                            case ')':
                                inBracket = false;
                                break;
                            case ';':
                                inComment = true;
                                break;
                        }
                    }

                    combinedRdata += c;
                    if (!inComment && c != '(' && c != ')')
                    {
                        parsedRdata += c;
                    }
                }

                if (inBracket)
                {
                    rdata = sr.ReadLine().Trim();
                    combinedRdata += "\n\t\t\t\t";
                }
            } while (inBracket);

            parsedRdata = parsedRdata.Trim();

            string fqRecordName = null;
            if (this.currentOrigin == ".")
            {
                fqRecordName = name.TrimEnd('.') + ".";
            }
            else
            {
                fqRecordName = (name + "." + this.currentOrigin).TrimStart('.');
            }
            
            if (name.EndsWith("."))
            {
                fqRecordName = name;
            }
            
            string relativeRecordName;
            if (fqRecordName == this.zoneName)
            {
                relativeRecordName = "@";
            }
            else
            {
                relativeRecordName = fqRecordName.Substring(0, fqRecordName.Length - this.zoneName.Length - 1);
            }

            parsedRecords.Add(
                new Record
                {
                    Name = relativeRecordName,
                    Class = Enum.Parse<RecordClass>(rrClass),
                    TimeToLive = ttl,
                    Type = Enum.Parse<RecordType>(type),
                    Value = parsedRdata,
                });
        }

        return parsedRecords;
    }
    
    private bool ParseDirective(string data, string directive, out string? value)
    {
        if (data.StartsWith(directive))
        {
            var directiveData = data.Substring(directive.Length).Trim();
    
            if (directiveData.Contains(';'))
            {
                directiveData = directiveData.Substring(0, directiveData.IndexOf(';'));
            }
    
            Console.WriteLine("NEW {1}: {0}", directiveData, directive);
    
            value = directiveData;
            return true;
        }
    
        value = null;
        return false;
    }
}