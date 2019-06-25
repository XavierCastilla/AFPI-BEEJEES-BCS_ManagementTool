using System;
using System.IO;

namespace afpiLog
{
    /// <summary>
    /// Provides methods for creating, reading and writing to a text log file.
    /// </summary>
    public class Logger
    {
        // const disallows changes after the var has been initializied
        private string filename = "log" + DateTime.Now.ToString() +".txt";

        // makes sure that the value of _path is only set at the declaration or consructor of this class
        private readonly string _path;

        /// <summary>
        /// Creates a blank log file at the directory where the executable is.
        /// </summary>
        public Logger()
        {
            File.CreateText(filename).Close();
            _path = filename;
        }

        /// <summary>
        /// Creates a blank log file at the specified directory.
        /// </summary>
        /// <param name="path">The directory where the log file should be created.</param>
        public Logger(string path)
        {
            File.CreateText(path).Close();
            _path = path;
        }

        /// <summary>
        /// Writes text to the log file.
        /// </summary>
        /// <param name="message">The message/text to write to the log file.</param>
        /// <returns>Returns a boolean flag of true to indicate that writing has been successful.</returns>
        public bool Write(string message)
        {
            try
            {
                string currLog = Read();
                File.WriteAllText(_path, currLog + message + Environment.NewLine);
                return true;
            }
            catch (Exception e)
            {
                e.ToString();
                throw;
            }
        }

        /// <summary>
        /// Reads the entire contents of the log file.
        /// </summary>
        /// <returns>Returns the contents of the log file as a single string.</returns>
        public string Read()
        {
            try
            {
                return File.ReadAllText(_path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
