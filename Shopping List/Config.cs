using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ShoppingList
{
	class Config
	{
        private bool autoSave;
        private bool autoPrice;
        private String opener;
        private String endingMessage;
        private String separator;

        public Config()
        {
            autoSave = false;
            autoPrice = false;
            opener = "";
            endingMessage = "";
            separator = ",";
        }

        public bool saveConfigFile(String filename)
        {
            StreamWriter file;

            try
            {
                file = new System.IO.StreamWriter(filename);
            }
            catch (IOException e)
            {
                //should do some kind of error logging
                return false;
            }

            file.WriteLine("autosave " + Convert.ToString(autoSave));
            file.WriteLine("autoprice " + Convert.ToString(autoPrice));
            file.WriteLine("opener " + opener);
            file.WriteLine("endingmessage " + endingMessage);
            file.WriteLine("separator " + separator);

            file.Close();
            return true;
        }

        public bool loadConfigFile(String filename)
        {
            StreamReader file;

            try
            {
                file = new System.IO.StreamReader(filename);
            }
            catch (IOException e)
            {
                //should do some kind of error logging
                return false;
            }

            String line = "";

            while ((line = file.ReadLine()) != null)
            {
                String[] lineInfo = line.Split(' ');

                if (lineInfo[0].Equals("autosave"))
                {
                    if (lineInfo.Length <= 1)
                    {
                        autoSave = false;
                    }
                    else
                    {
                        autoSave = Convert.ToBoolean(lineInfo[1]);
                    }
                    
                }
                else if (lineInfo[0].Equals("autoprice"))
                {
                    if (lineInfo.Length <= 1)
                    {
                        autoPrice = false;
                    }
                    else
                    {
                        autoPrice = Convert.ToBoolean(lineInfo[1]);
                    }
                }
                else if (lineInfo[0].Equals("opener"))
                {
                    if (lineInfo.Length <= 1)
                    {
                        opener = "";
                    }
                    else
                    {
                        opener = lineInfo[1];
                    }
                }
                else if (lineInfo[0].Equals("endingmessage"))
                {
                    if (lineInfo.Length <= 1)
                    {
                        endingMessage = "";
                    }
                    else
                    {
                        String ending = "";

                        for (int i = 1; i < lineInfo.Length; i++)
                        {
                            ending += lineInfo[i];
                        }

                        endingMessage = ending;
                    }
                }
                else if (lineInfo[0].Equals("separator"))
                {
                    if (lineInfo.Length <= 1)
                    {
                        separator = "";
                    }
                    else
                    {
                        separator = lineInfo[1];
                    }
                }
                else
                {
                }
            }

            file.Close();
            return true;
        }

        public bool AutoSave
        {
            get
            {
                return autoSave;
            }
            set
            {
                autoSave = value;
            }
        }

        public bool AutoPrice
        {
            get
            {
                return autoPrice;
            }
            set
            {
                autoPrice = value;
            }
        }

        public String Opener
        {
            get
            {
                return opener;
            }
            set
            {
                opener = value;
            }
        }

        public String EndingMessage
        {
            get
            {
                return endingMessage;
            }
            set
            {
                endingMessage = value;
            }
        }

        public String Separator
        {
            get
            {
                return separator;
            }
            set
            {
                separator = value;
            }
        }
	}    
}
