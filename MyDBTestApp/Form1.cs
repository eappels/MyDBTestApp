using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MyDBTestApp
{
    public partial class frm_Main : Form
    {

        private List<PersonModel> people = new List<PersonModel>();

        public frm_Main()
        {
            InitializeComponent();
            LoadPeopleList();
        }

        private void LoadPeopleList()
        {
            people = SqlLiteDataAccess.LoadAllPeople();
            WrireUpPeopleList();
        }

        private void WrireUpPeopleList()
        {
            lst_Output.DataSource = null;
            lst_Output.DataSource = people;
            lst_Output.DisplayMember = "Fullname";
            lst_Output.SelectedIndex = -1;
        }

        private void btn_AddPerson_Click(object sender, EventArgs e)
        {
            if (txt_FirstName.Text.Length > 0 && txt_LastName.Text.Length > 0)
            {
                PersonModel personmodel = new PersonModel();
                personmodel.Firstname = txt_FirstName.Text;
                personmodel.LastName = txt_LastName.Text;
                SqlLiteDataAccess.SavePerson(personmodel);
                txt_FirstName.Text = "";
                txt_LastName.Text = "";
                LoadPeopleList();
            }
        }

        private void btn_Edit_Click(object sender, EventArgs e)
        {
            if (lst_Output.SelectedItem == null) return;
            PersonModel person = SqlLiteDataAccess.LoadPerson(lst_Output.SelectedIndex);
            txt_FirstName.Text = person.Firstname;
            txt_LastName.Text = person.LastName;
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (lst_Output.SelectedItems.Count > 0)
            {
                foreach (PersonModel model in lst_Output.SelectedItems)
                {
                    SqlLiteDataAccess.DeletePerson(model.Id);
                }
                LoadPeopleList();
            }
        }
    }
}