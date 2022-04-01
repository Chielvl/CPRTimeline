using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using TimeLine;

public class ManageFile
{
    private static ManageFile instance;

    private SortedDictionary<string, Event> eventsDictionary;
    private BinaryFormatter formatter;
    public MainWindow mainWindow;

    private const string DATA_FILENAME = "eventsinformation.dat";

    public static ManageFile GetInstance()
    {
        if (instance == null)
        {
            instance = new ManageFile();
        } // end if

        return instance;
    } // end public static FriendsInformation Instance()

    private ManageFile()
    {
        // Create a Dictionary to store events at runtime
        this.eventsDictionary = new SortedDictionary<string, Event>();
        this.formatter = new BinaryFormatter();
    } // end private FriendsInformation()

    public void AddEvent(Event e)
    {
        // If we already had added a event with this name, add description to event
        if (this.eventsDictionary.ContainsKey(e.GetDate()))
        {
            if (System.Windows.Forms.MessageBox.Show($"Er bestaat al een event met de datum {e.GetDate()}." +
            $" Wil je deze gebeurtenis toevoegen aan dat event?", "Event bestaat al",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                eventsDictionary.TryGetValue(e.GetDate(), out Event presentEvent);
                presentEvent.AddDescription(e.GetDescription());
            }
        }
        // Else if we do not have this event details 
        // in our dictionary
        else
        {
            // Add him in the dictionary
            this.eventsDictionary.Add(e.GetDate(), e);
        } // end if
    } // end public bool AddFriend(string name, string email)

    public void RemoveEvent(string date)
    {

        if (System.Windows.Forms.MessageBox.Show($"Weet je zeker dat je de entry met datum {date} wilt verwijderen?", "Weet je het zeker", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
        { // If we do not have a event with this name
            if (!this.eventsDictionary.ContainsKey(date))
            {
                Console.WriteLine($"Entry met datum {date} is nog niet toegevoegd.");
            }
            // Else if we have a event with this name
            else
            {
                if (this.eventsDictionary.Remove(date))
                {
                    Console.WriteLine($"Entry met datum {date} is verwijderd.");
                }
                else
                {
                    Console.WriteLine($"Het is niet mogelijk {date} te verwijderen");
                } // end if
            } // end if
            Save();
            Load();
        }
    } // end public bool RemoveFriend(string name)

    public void Save()
    {
        FileStream writerFileStream =
        new FileStream(DATA_FILENAME, FileMode.Create, FileAccess.Write);
        // Gain code access to the file that we are going
        // to write to
        try
        {
            // Create a FileStream that will write data to file.

            // Save our dictionary of events to file
            this.formatter.Serialize(writerFileStream, this.eventsDictionary);

            // Close the writerFileStream when we are done.
            writerFileStream.Close();
            MessageBox.Show("De tijdlijn is geupdate", "Update gelukt");
            mainWindow.ClearEntryField(mainWindow.EventDesc);
        }
        catch (Exception e)
        {
            MessageBox.Show("Het is niet gelukt om de gebeurtenis op te slaan.\n\n" + e);
        } // end try-catch
        finally
        {
            writerFileStream.Close();
        }
    } // end public bool Load()

    public void Load()
    {

        // Check if we had previously Save information of our events
        // previously
        if (File.Exists(DATA_FILENAME))
        {
            try
            {
                // Create a FileStream will gain read access to the 
                // data file.
                FileStream readerFileStream = new FileStream(DATA_FILENAME,
                FileMode.Open, FileAccess.Read);
                // Reconstruct information of our events from file.
                this.eventsDictionary = (SortedDictionary<string, Event>)this.formatter.Deserialize(readerFileStream);
                // Close the readerFileStream when we are done
                readerFileStream.Close();
                eventsDictionary.OrderBy(key => key.Key);
                eventsDictionary.Reverse();

                mainWindow.ClearTimeLine();
                if (this.eventsDictionary.Keys.Count > 0)
                {
                    foreach (var key in eventsDictionary.Reverse())
                    {
                        this.eventsDictionary.TryGetValue(key.Key, out Event e);

                        mainWindow.AddItemToTimeLine(e);

                    }
                }
                else
                    mainWindow.AddItemToTimeLine(new Event(DateTime.Today, "Er is geen event gevonden"));


            }
            catch (Exception e)
            {
                MessageBox.Show("Het bestand bestaat, maar er is een probleem met het uitlezen.\n" + e);
            } // end try-catch
        } // end if
    } // end public bool Load()

    public SortedDictionary<string, Event> GetEventDict()
    {
        Load();
        return this.eventsDictionary;
    }
} // end public class FriendsInformation