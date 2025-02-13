﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Racelogic.Geodetics;
using Racelogic.Gnss;
using Racelogic.Gnss.SatGen;

namespace satgen2
{
    public class ConfigFile
    {
        private TimeSpan timeOfDay;

        private int day;

        private int month;

        private int year;

        private SignalType[] signalTypes;

        public string AlmanacFile
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            protected set;
        }

        public string GpsAlmanacFile
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            protected set;
        }

        public string GlonassAlmanacFile
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            protected set;
        }

        public string BeiDouAlmanacFile
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            protected set;
        }

        public string GalileoAlmanacFile
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            protected set;
        }

        public string NavicAlmanacFile
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            protected set;
        }

        public DateTime Date => new DateTime(year, month, day) + timeOfDay;

        public GravitationalModel GravitationalModel
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            protected set;
        }

        public double? Mask
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            protected set;
        }

        public string NmeaFile
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            protected set;
        }

        public string OutputFile
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            protected set;
        }

        public uint BitsPerSample
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            protected set;
        }

        public bool Rinex
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            set;
        }

        public double Attenuation
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            protected set;
        }

        public IReadOnlyList<SignalType> SignalTypes
        {
            [DebuggerStepThrough]
            get
            {
                return signalTypes;
            }
        }

        public int SampleRate
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            protected set;
        }

        public static ConfigFile Read(string filename)
        {
            ConfigFile configFile = new ConfigFile();
            using (StreamReader streamReader = new StreamReader(filename, detectEncodingFromByteOrderMarks: true))
            {
                string text;
                while ((text = streamReader.ReadLine()) != null)
                {
                    string[] array = text.Split('=');
                    if (array.Length >= 2)
                    {
                        string text2 = array[1].Trim();
                        string text3 = array[1].ToLowerInvariant().Trim();
                        switch (array[0].ToLowerInvariant().Trim())
                        {
                            case "source_nmea_file":
                                configFile.NmeaFile = text2;
                                break;
                            case "signal":
                                configFile.signalTypes = (from nst in ((IEnumerable<string>)text3.Split(new char[1]
                                    {
                                        ','
                                    })).Where((Func<string, bool>)((string s) => !string.IsNullOrWhiteSpace(s))).Select((Func<string, string>)((string s) => s.Trim())).Select((Func<string, SignalType?>)delegate(string s)
                                    {
                                        foreach (SignalType value1 in Enum.GetValues(typeof(SignalType)))
                                        {
                                            if (s == value1.ToCodeName().ToLowerInvariant())
                                            {
                                                return value1;
                                            }
                                        }
                                        return null;
                                    })
                                    where nst.HasValue
                                    select nst.Value).SelectMany((Func<SignalType, IEnumerable<SignalType>>)((SignalType st) => Signal.GetIndividualSignalTypes(st))).Distinct().ToArray();
                                break;
                            case "gps_start_time_in_sec":
                            {
                                if (double.TryParse(text2, NumberStyles.Integer, CultureInfo.InvariantCulture, out double result4))
                                {
                                    configFile.timeOfDay = TimeSpan.FromSeconds(result4);
                                }
                                break;
                            }
                            case "simulation_day":
                                int.TryParse(text2, NumberStyles.Integer, CultureInfo.InvariantCulture, out configFile.day);
                                break;
                            case "simulation_mon":
                                int.TryParse(text2, NumberStyles.Integer, CultureInfo.InvariantCulture, out configFile.month);
                                break;
                            case "simulation_year":
                                int.TryParse(text2, NumberStyles.Integer, CultureInfo.InvariantCulture, out configFile.year);
                                break;
                            case "almanac_file":
                                configFile.AlmanacFile = text2;
                                configFile.GpsAlmanacFile = text2;
                                break;
                            case "almanac_file_gps":
                                configFile.GpsAlmanacFile = text2;
                                break;
                            case "almanac_file_glo":
                                configFile.GlonassAlmanacFile = text2;
                                break;
                            case "almanac_file_bds":
                                configFile.BeiDouAlmanacFile = text2;
                                break;
                            case "almanac_file_gal":
                                configFile.GalileoAlmanacFile = text2;
                                break;
                            case "almanac_file_nav":
                                configFile.NavicAlmanacFile = text2;
                                break;
                            case "mask_in_deg":
                            case "mask_deg":
                            {
                                if (double.TryParse(text2, NumberStyles.Integer, CultureInfo.InvariantCulture, out double result5))
                                {
                                    configFile.Mask = result5 / 180.0 * Math.PI;
                                }
                                break;
                            }
                            case "if_iq_file":
                            case "iq_outputfile":
                                configFile.OutputFile = text2;
                                break;
                            case "bitpersample":
                            {
                                if (!uint.TryParse(text2, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint result3))
                                {
                                    result3 = 1u;
                                }
                                configFile.BitsPerSample = result3;
                                break;
                            }
                            case "egm":
                            {
                                ConfigFile configFile2 = configFile;
                                GravitationalModel gravitationalModel;
                                switch (text3)
                                {
                                    case "egm84":
                                        gravitationalModel = GravitationalModel.Egm84;
                                        break;
                                    case "egm96":
                                        gravitationalModel = GravitationalModel.Egm96;
                                        break;
                                    case "egm2008":
                                        gravitationalModel = GravitationalModel.Egm2008;
                                        break;
                                    case "nmea file":
                                        gravitationalModel = GravitationalModel.Nmea;
                                        break;
                                    default:
                                        gravitationalModel = GravitationalModel.Wgs84;
                                        break;
                                }
                                configFile2.GravitationalModel = gravitationalModel;
                                break;
                            }
                            case "rinex":
                                configFile.Rinex = "yes".Equals(text3);
                                break;
                            case "attenuation":
                            {
                                if (double.TryParse(text2, NumberStyles.Float, CultureInfo.InvariantCulture, out double result2))
                                {
                                    configFile.Attenuation = result2;
                                }
                                break;
                            }
                            case "samplerate_in_khz":
                            {
                                if (int.TryParse(text2, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
                                {
                                    configFile.SampleRate = 1000 * result;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            if (configFile.signalTypes == null || !configFile.signalTypes.Any())
            {
                configFile.signalTypes = new SignalType[1]
                {
                    SignalType.GpsL1CA
                };
            }
            return configFile;
        }
    }
}