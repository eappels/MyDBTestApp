using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MyDBTestApp
{
    public partial class Form1 : Form
    {

        private List<PersonModel> people = new List<PersonModel>();

        public Form1()
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
        }

        private void btn_AddPerson_Click(object sender, EventArgs e)
        {
            PersonModel personmodel = new PersonModel();
            personmodel.Firstname = txt_FirstName.Text;
            personmodel.LastName = txt_LastName.Text;
            SqlLiteDataAccess.SavePerson(personmodel);
            txt_FirstName.Text = "";
            txt_LastName.Text = "";
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            LoadPeopleList();
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (lst_Output.SelectedItems.Count > 0)
            {
                foreach (PersonModel model in lst_Output.SelectedItems)
                {
                    SqlLiteDataAccess.DeletePerson(model.Id);
                }
            }
        }
    }
}