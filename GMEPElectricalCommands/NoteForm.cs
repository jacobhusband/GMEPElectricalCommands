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
      this.notesStorage = userInterface.getNotesStorage();
      append_notes_to_textbox();

      // Add event listener for form closing
      this.FormClosing += new FormClosingEventHandler(this.noteForm_FormClosing);
    }

    private void append_notes_to_textbox()
    {
      foreach (var key in this.notesStorage.Keys)
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

    private void noteForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      update_notes_to_match_the_note_form();
    }

    public void update_notes_to_match_the_note_form()
    {
      // Split the text in the NOTES_TEXTBOX by newlines and add any new notes to notesStorage
      var notes = NOTES_TEXTBOX.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
      foreach (var note in notes)
      {
        if (!string.IsNullOrEmpty(note) && !this.notesStorage.ContainsKey(note))
        {
          this.notesStorage.Add(note, new List<int>());
        }
      }

      // Remove any notes from notesStorage that are not in the NOTES_TEXTBOX
      var notesToRemove = new List<string>();
      foreach (var key in this.notesStorage.Keys)
      {
        if (!NOTES_TEXTBOX.Text.Contains(key))
        {
          notesToRemove.Add(key);
        }
      }

      foreach (var note in notesToRemove)
      {
        this.notesStorage.Remove(note);
      }

      // Call the update_notes_storage method on the userInterface
      this.userInterface.update_notes_storage(this.notesStorage);
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