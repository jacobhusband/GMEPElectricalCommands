using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMEPElectricalCommands
{
  public partial class noteForm : Form
  {
    private UserInterface userInterface;
    private Dictionary<string, List<int>> notesStorage;

    public noteForm(UserInterface userInterface)
    {
      InitializeComponent();

      this.userInterface = userInterface;
      this.noteForm_Load(null, null);

      // Add event listener for form closing
      this.FormClosing += new FormClosingEventHandler(this.noteForm_FormClosing);

      // Add event listener for form opening
      this.Load += new EventHandler(this.noteForm_Load);
    }

    public void print_notes_storage(Dictionary<string, List<int>> notesStorage)
    {
      foreach (var note in notesStorage)
      {
        Console.WriteLine($"Key: {note.Key}, Values: {string.Join(", ", note.Value)}");
      }
    }

    private void noteForm_Load(object sender, EventArgs e)
    {
      var notesStorage = userInterface.getNotesStorage();
      print_notes_storage(notesStorage);
      append_notes_to_textbox(notesStorage);
    }

    private void append_notes_to_textbox(Dictionary<string, List<int>> notesStorage)
    {
      // Check if notesStorage is not null and has keys before running the foreach loop
      if (notesStorage != null && notesStorage.Keys.Count > 0)
      {
        foreach (var key in notesStorage.Keys)
        {
          // Check if the NOTES_TEXTBOX does not contain the key before appending it
          if (!NOTES_TEXTBOX.Text.Contains(key))
          {
            // Check if the last line is not empty, if so, append a newline first
            if (!string.IsNullOrEmpty(NOTES_TEXTBOX.Lines.LastOrDefault()))
            {
              NOTES_TEXTBOX.AppendText(Environment.NewLine);
            }
            NOTES_TEXTBOX.AppendText(key + Environment.NewLine);
          }
        }
      }
    }

    private void noteForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      var notesStorage = userInterface.getNotesStorage();
      update_notes_to_match_the_note_form(notesStorage);
    }

    public void update_notes_to_match_the_note_form(Dictionary<string, List<int>> notesStorage)
    {
      if (notesStorage == null || notesStorage.Keys.Count == 0)
      {
        return;
      }

      // Split the text in the NOTES_TEXTBOX by newlines and add any new notes to notesStorage
      var notes = NOTES_TEXTBOX.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
      foreach (var note in notes)
      {
        if (!string.IsNullOrEmpty(note) && !notesStorage.ContainsKey(note))
        {
          notesStorage.Add(note, new List<int>());
        }
      }

      // Remove any notes from notesStorage that are not in the NOTES_TEXTBOX
      var notesToRemove = new List<string>();

      foreach (var key in notesStorage.Keys)
      {
        if (!NOTES_TEXTBOX.Text.Contains(key))
        {
          notesToRemove.Add(key);
        }
      }

      foreach (var note in notesToRemove)
      {
        notesStorage.Remove(note);
      }

      // Call the update_notes_storage method on the userInterface
      this.userInterface.update_notes_storage(notesStorage);
    }

    private void ADD_NOTE_BUTTON_Click(object sender, EventArgs e)
    {
      // Check if the selected item in the QUICK_ADD_COMBOBOX is not null
      if (QUICK_ADD_COMBOBOX.SelectedItem != null)
      {
        var selectedItem = QUICK_ADD_COMBOBOX.SelectedItem.ToString();
        // Check if the NOTES_TEXTBOX does not already contain the selected item
        if (!NOTES_TEXTBOX.Text.Contains(selectedItem))
        {
          // Add the selected item on a newline to the NOTES_TEXTBOX
          NOTES_TEXTBOX.AppendText(selectedItem + Environment.NewLine);
        }
      }
    }
  }
}