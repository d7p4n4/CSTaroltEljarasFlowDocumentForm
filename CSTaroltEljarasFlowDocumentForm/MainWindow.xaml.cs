using CSAc4yObjectObjectService.Association;
using CSAc4yObjectObjectService.Object;
using CSAc4yUtilityContainer;
using Modul.Final.Class;
using Modul.PersistentService.Class;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CSTaroltEljarasFlowDocumentForm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string APPSETTING_SQLCONNECTIONSTRING = ConfigurationManager.AppSettings["sqlConnectionString"];
        public Dictionary<string, TaroltEljaras> TaroltEljarasDictionary = new Dictionary<string, TaroltEljaras>();
        public TaroltEljarasKontener Kontener = new TaroltEljarasKontener()
        {
            Lista = new List<TaroltEljaras>()
        };
        public List<MuveletAzonosito> MuveletLista = new List<MuveletAzonosito>();
        public int SectionSorszam = 1;
        public int MuveletSorszam = 1;

        Ac4yUtility Ac4yUtility = new Ac4yUtility();

        public MainWindow()
        {
            InitializeComponent();
        }


        private void AddTextBox(object subject, RoutedEventArgs e)
        {
            AddTaroltEljarasForm("", "", "", "");
        }

        private void ButtonAction(object subject, RoutedEventArgs e)
        {
            Kontener.Lista.Clear();

            foreach (var block in uiFlowDocument.Blocks)
            {
                foreach (var dictionary in TaroltEljarasDictionary)
                {
                    if (block.Name.Equals(dictionary.Key))
                    {
                        Section sectionBlock = (Section)block;
                        BlockUIContainer uiContainmer = (BlockUIContainer)sectionBlock.Blocks.FirstBlock;
                        WrapPanel uiMainWrapPanel = (WrapPanel)uiContainmer.Child;
                        UIElementCollection uiInnerWrapPanels = uiMainWrapPanel.Children;
                        foreach (var uiInnerWrapPanel in uiInnerWrapPanels)
                        {
                            UIElementCollection uiWrapPanelElements = uiMainWrapPanel.Children;
                            foreach (var element in uiWrapPanelElements)
                            {
                                if (element.GetType().Name.Equals("TextBox"))
                                {
                                    TextBox uiTextBox = (TextBox)element;
                                    if (uiTextBox.Name.Equals("uiTextBoxMegnevezes"))
                                    {
                                        dictionary.Value.Megnevezes = uiTextBox.Text;
                                    }
                                    else if (uiTextBox.Name.Equals("uiTextBoxKod"))
                                    {
                                        dictionary.Value.Kod = uiTextBox.Text;
                                    }
                                    else if (uiTextBox.Name.Equals("uiTextBoxNev"))
                                    {
                                        dictionary.Value.Nev = uiTextBox.Text;
                                    }
                                    else if (uiTextBox.Name.Equals("uiTextBoxLeiras"))
                                    {
                                        dictionary.Value.Leiras = uiTextBox.Text;
                                    }
                                }

                                if (element.GetType().Name.Equals("WrapPanel"))
                                {
                                    WrapPanel wrapPanel = (WrapPanel)element;
                                    if (wrapPanel.Name.StartsWith("muvelet"))
                                    {
                                        foreach (MuveletAzonosito muveletAzonosito in MuveletLista)
                                        {
                                            if (wrapPanel.Name.Equals(muveletAzonosito.WrapPanelNev) && sectionBlock.Name.Equals(muveletAzonosito.SectionNev))
                                            {
                                                foreach (var elem in wrapPanel.Children)
                                                {
                                                    if (elem.GetType().Name.Equals("TextBox"))
                                                    {
                                                        TextBox uiTextBox = (TextBox)elem;
                                                        if (uiTextBox.Name.Equals("uiTextBoxNev"))
                                                        {
                                                            muveletAzonosito.Muvelet.Nev = uiTextBox.Text;
                                                        }
                                                        else if (uiTextBox.Name.Equals("uiTextBoxLeiras"))
                                                        {
                                                            muveletAzonosito.Muvelet.Leiras = uiTextBox.Text;
                                                        }
                                                        if (uiTextBox.Name.Equals("uiTextBoxKod"))
                                                        {
                                                            muveletAzonosito.Muvelet.Kod = uiTextBox.Text;
                                                        }
                                                        else if (uiTextBox.Name.Equals("uiTextBoxMegnevezes"))
                                                        {
                                                            muveletAzonosito.Muvelet.Megnevezes = uiTextBox.Text;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (var dictionary in TaroltEljarasDictionary)
            {
                dictionary.Value.MuveletLista.Clear();
                foreach (MuveletAzonosito muveletAzonosito in MuveletLista)
                {
                    if (dictionary.Key.Equals(muveletAzonosito.SectionNev))
                    {
                        dictionary.Value.MuveletLista.Add(muveletAzonosito.Muvelet);
                    }
                }
            }
            foreach (var dictionary in TaroltEljarasDictionary)
            {
                Kontener.Lista.Add(dictionary.Value);
            }

            uiTextBlock.Text = Ac4yUtility.GetAsXml(Kontener);

            //UploadTanuloKontener(tanuloKontener);
            //UploadKontenerTanuloAssociation(tanuloKontener);
            
        }

        public void XmlBetoltes(object subject, RoutedEventArgs e)
        {
            OpenXML();

            List<Block> flowDocumentBlockList = uiFlowDocument.Blocks.ToList();
            uiFlowDocument.Blocks.Clear();

            foreach (var block in flowDocumentBlockList)
            {
                if (block.Name.Equals("section0"))
                {
                    uiFlowDocument.Blocks.Add(block);
                }
            }

            SectionSorszam = 1;
            MuveletSorszam = 1;

            TaroltEljarasDictionary.Clear();
            Kontener.Lista.Clear();

            string xml = uiTextBlock.Text;
            TaroltEljarasKontener kontenerUj =  (TaroltEljarasKontener)Ac4yUtility.Xml2Object(xml, typeof(TaroltEljarasKontener));
            int x = 0;
            if (kontenerUj != null)
            {
                foreach (var taroltEljaras in kontenerUj.Lista)
                {
                    AddTaroltEljarasForm(taroltEljaras.Megnevezes, taroltEljaras.Nev, taroltEljaras.Kod, taroltEljaras.Leiras);
                    foreach (Muvelet muvelet in taroltEljaras.MuveletLista)
                    {
                        AddMuveletForm(muvelet.Megnevezes, muvelet.Nev, muvelet.Kod, muvelet.Leiras, "section" + (SectionSorszam - 1));
                    }
                }
            }
        }

        public void AddMuveletButton(object subject, RoutedEventArgs e)
        {
            Button button = subject as Button;
            string sectionName = button.Tag.ToString();

            AddMuveletForm("", "", "", "", sectionName);
        }

        public void AddMuveletForm(string megnevezes, string nev, string kod, string leiras, string sectionName)
        {

            string name = "muveletWrapPanel" + MuveletSorszam;

            WrapPanel uiInnerWrapPanel1 = new WrapPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness()
                {
                    Top = 12,
                    Left = 25
                },
                Background = new SolidColorBrush(Color.FromRgb(220, 220, 255)),
                Name = name
            };
            
            uiInnerWrapPanel1.Children.Add(
                new Label()
                {
                    Content = "Megnevezés: ",
                    Width = 100
                });

            uiInnerWrapPanel1.Children.Add(
                new TextBox()
                {
                    Name = "uiTextBoxMegnevezes",
                    Width = 250,
                    Height = 25,
                    Text = megnevezes
                });

            uiInnerWrapPanel1.Children.Add(
                new Label()
                {
                    Content = "Név: ",
                    Width = 100
                });

            uiInnerWrapPanel1.Children.Add(
                new TextBox()
                {
                    Name = "uiTextBoxNev",
                    Width = 250,
                    Height = 25,
                    Text = nev
                });

            uiInnerWrapPanel1.Children.Add(
                new Label()
                {
                    Content = "Kód: ",
                    Width = 100
                });

            uiInnerWrapPanel1.Children.Add(
                new TextBox()
                {
                    Name = "uiTextBoxKod",
                    Width = 250,
                    Height = 25,
                    Text = kod
                });

            uiInnerWrapPanel1.Children.Add(
                new Label()
                {
                    Content = "Leírás: ",
                    Width = 100
                });

            uiInnerWrapPanel1.Children.Add(
                new TextBox()
                {
                    Name = "uiTextBoxLeiras",
                    Width = 250,
                    Height = 25,
                    Text = leiras
                });
            Button uiTorlesButton = new Button()
            {
                Width = 150,
                Content = "Művelet törlése",
                Tag = uiInnerWrapPanel1.Name + "," + sectionName,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness()
                {
                    Top = 12,
                    Left = 25
                },
            };

            uiTorlesButton.Click += deleteMuvelet;

            foreach (var block in uiFlowDocument.Blocks)
            {
                if (block.Name.Equals(sectionName))
                {
                    Section sectionBlock = (Section)block;
                    BlockUIContainer uiContainmer = (BlockUIContainer)sectionBlock.Blocks.FirstBlock;
                    WrapPanel uiMainWrapPanel = (WrapPanel)uiContainmer.Child;

                    uiInnerWrapPanel1.Children.Add(uiTorlesButton);
                    uiMainWrapPanel.Children.Add(uiInnerWrapPanel1);

                    MuveletAzonosito muveletAzonosito = new MuveletAzonosito()
                    {
                        SectionNev = sectionName,
                        WrapPanelNev = name,
                        Muvelet = new Muvelet()
                    };

                    MuveletLista.Add(muveletAzonosito);
                }
            }
            MuveletSorszam++;
            //SectionSorszam++;


        }
        
        public void UploadTanuloKontener(TaroltEljarasKontener kontener)
        {
            SqlConnection sqlConnection = new SqlConnection(APPSETTING_SQLCONNECTIONSTRING);
            Ac4yXMLObjectPersistentService ac4YXMLObjectPersistentService = new Ac4yXMLObjectPersistentService();

            sqlConnection.Open();

            foreach (var taroltEljaras in kontener.Lista)
            {

                foreach (Muvelet muvelet in taroltEljaras.MuveletLista)
                {
                    SetByNamesResponse setByNamesResponseMuvelet =
                        new Ac4yObjectObjectService(sqlConnection).SetByNames(
                            new SetByNamesRequest()
                            {
                                TemplateName = "Metódus",
                                Name = muvelet.Leiras,
                            });

                    string muveletXml = Ac4yUtility.GetAsXml(muvelet);
                    string muveletGuid = setByNamesResponseMuvelet.Ac4yObject.GUID;

                    ac4YXMLObjectPersistentService.Save(new Ac4yXMLObject()
                    {
                        serialization = muveletXml,
                        GUID = muveletGuid
                    });

                }

                taroltEljaras.MuveletLista.Clear();
                
                SetByNamesResponse setByNamesResponseTaroltEljaras =
                    new Ac4yObjectObjectService(sqlConnection).SetByNames(
                        new SetByNamesRequest()
                        {
                            TemplateName = "tárolt eljárás",
                            Name = taroltEljaras.Megnevezes,
                        });

                string taroltEljarasXml = Ac4yUtility.GetAsXml(taroltEljaras);
                string taroltEljarasGuid = setByNamesResponseTaroltEljaras.Ac4yObject.GUID;

                ac4YXMLObjectPersistentService.Save(new Ac4yXMLObject()
                {
                    serialization = taroltEljarasXml,
                    GUID = taroltEljarasGuid
                });
            }

            kontener.Lista.Clear();

            SetByNamesResponse setByNamesResponse =
                new Ac4yObjectObjectService(sqlConnection).SetByNames(
                    new SetByNamesRequest()
                    {
                        TemplateName = "tárolt eljárás konténer",
                        Name = "Teszt3"
                    });

            string xml = Ac4yUtility.GetAsXml(kontener);
            string guid = setByNamesResponse.Ac4yObject.GUID;
            ac4YXMLObjectPersistentService.Save(new Ac4yXMLObject()
            {
                serialization = xml,
                GUID = guid
            });
        }

        public void UploadKontenerTanuloAssociation(TaroltEljarasKontener kontener)
        {
            SqlConnection sqlConnection = new SqlConnection(APPSETTING_SQLCONNECTIONSTRING);
            sqlConnection.Open();

            foreach (var taroltEljaras in kontener.Lista)
            {
                Ac4yAssociationObjectService.SetByNamesResponse setByNamesResponse =
                    new Ac4yAssociationObjectService(sqlConnection).SetByNames(
                        new Ac4yAssociationObjectService.SetByNamesRequest()
                        {
                            AssociationPathName = "tárolt eljárás konténer.tárolt eljárás",
                            OriginTemplateName = "tárolt eljárás konténer",
                            OriginName = "Teszt3",
                            TargetTemplateName = "tárolt eljárás",
                            TargetName = taroltEljaras.Megnevezes
                        });

                foreach (Muvelet muvelet in taroltEljaras.MuveletLista)
                {
                    Ac4yAssociationObjectService.SetByNamesResponse setByNamesResponseNyelv =
                    new Ac4yAssociationObjectService(sqlConnection).SetByNames(
                        new Ac4yAssociationObjectService.SetByNamesRequest()
                        {
                            AssociationPathName = "tárolt eljárás.Metódus",
                            OriginTemplateName = "tárolt eljárás",
                            OriginName = taroltEljaras.Megnevezes,
                            TargetTemplateName = "Metódus",
                            TargetName = muvelet.Leiras
                        });
                }                
            }
        }

        private void AddTaroltEljarasForm(object subject, RoutedEventArgs e)
        {
            AddTaroltEljarasForm("", "", "", "");
        }

        public void AddTaroltEljarasForm(string megnevezes, string nev, string kod, string leiras)
        {
            WrapPanel uiInnerWrapPanel1 = new WrapPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness()
                {
                    Top = 12,
                    Left = 25
                }
            };

            Section sectionFromCode = new Section()
            {
                Background = new SolidColorBrush(Color.FromRgb(248, 248, 255)),
                Name = "section" + SectionSorszam
            };

            uiInnerWrapPanel1.Children.Add(
                new Label()
                {
                    Content = "Megnevezés: ",
                    Width = 100
                });

            uiInnerWrapPanel1.Children.Add(
                new TextBox()
                {
                    Name = "uiTextBoxMegnevezes",
                    Width = 250,
                    Height = 25,
                    Text = megnevezes
                });

            uiInnerWrapPanel1.Children.Add(
                new Label()
                {
                    Content = "Név: ",
                    Width = 100
                });

            uiInnerWrapPanel1.Children.Add(
                new TextBox()
                {
                    Name = "uiTextBoxNev",
                    Width = 250,
                    Height = 25,
                    Text = nev
                });

            uiInnerWrapPanel1.Children.Add(
                new Label()
                {
                    Content = "Kód: ",
                    Width = 100
                });

            uiInnerWrapPanel1.Children.Add(
                new TextBox()
                {
                    Name = "uiTextBoxKod",
                    Width = 250,
                    Height = 25,
                    Text = kod
                });

            uiInnerWrapPanel1.Children.Add(
                new Label()
                {
                    Content = "Leírás: ",
                    Width = 100
                });

            uiInnerWrapPanel1.Children.Add(
                new TextBox()
                {
                    Name = "uiTextBoxLeiras",
                    Width = 250,
                    Height = 25,
                    Text = leiras
                });
            
            Button uiAddMuveletButton = new Button()
            {
                Width = 100,
                Height = 25,
                Content = "Művelet +",
                Margin = new Thickness()
                {
                    Top = 12,
                    Left = 25
                },
                HorizontalAlignment = HorizontalAlignment.Left,
                Tag = sectionFromCode.Name
            };

            uiAddMuveletButton.Click += AddMuveletButton;

            uiInnerWrapPanel1.Children.Add(uiAddMuveletButton);
            
            Button uiTorlesButton = new Button()
            {
                Width = 200,
                Height = 25,
                Margin = new Thickness()
                {
                    Left = 250
                },
                Content = "Törlés",
                Tag = sectionFromCode.Name
            };

            uiTorlesButton.Click += deleteButton;

            uiInnerWrapPanel1.Children.Add(uiTorlesButton);

            BlockUIContainer uiContainer = new BlockUIContainer()
            {
                Child = uiInnerWrapPanel1,
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                BorderThickness = new Thickness()
                {
                    Bottom = 1,
                    Left = 1,
                    Top = 1,
                    Right = 1
                }

            };

            uiTorlesButton.Click += deleteButton;

            sectionFromCode.Blocks.Add(uiContainer);
            uiFlowDocument.Blocks.Add(sectionFromCode);

            TaroltEljarasDictionary.Add(sectionFromCode.Name, new TaroltEljaras()
            {
                MuveletLista = new List<Muvelet>()
            });

            SectionSorszam++;

        }

        public void OpenXML()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                        uiTextBlock.Text = fileContent;
                    }
                }
            }
        }

        private void SaveXml(object subject, RoutedEventArgs e)
        {
            using (System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog())
            {
                // Displays a SaveFileDialog so the user can save the Image
                // assigned to Button2.

                saveFileDialog.InitialDirectory = "c:\\";
                saveFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                saveFileDialog.Title = "Save an Image File";
                saveFileDialog.ShowDialog();

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, uiTextBlock.Text);
                }
            }
        }

        private void deleteMuvelet(object subject, RoutedEventArgs e)
        {
            var clickedButton = subject as Button;
            string wrapName = clickedButton.Tag.ToString().Substring(0, clickedButton.Tag.ToString().IndexOf(","));
            string sectionName = clickedButton.Tag.ToString().Substring(clickedButton.Tag.ToString().IndexOf(",") + 1);

            foreach (var block in uiFlowDocument.Blocks)
            {
                if (block.Name.Equals(sectionName))
                {
                    Section sectionBlock = (Section)block;
                    BlockUIContainer uiContainmer = (BlockUIContainer)sectionBlock.Blocks.FirstBlock;
                    WrapPanel uiMainWrapPanel = (WrapPanel)uiContainmer.Child;
                    UIElementCollection uiInnerWrapPanels = uiMainWrapPanel.Children;
                    foreach (var uiInnerWrapPanel in uiInnerWrapPanels)
                    {

                        if (uiInnerWrapPanel.GetType().Name.Equals("WrapPanel"))
                        {
                            WrapPanel wrapPanel = (WrapPanel)uiInnerWrapPanel;
                            if (wrapPanel.Name.Equals(wrapName))
                            {
                                uiMainWrapPanel.Children.Remove(wrapPanel);
                                break;
                            }
                        }

                    }
                }

            }

            foreach (MuveletAzonosito muveletAzonosito in MuveletLista)
            {
                if (muveletAzonosito.SectionNev.Equals(sectionName) && muveletAzonosito.WrapPanelNev.Equals(wrapName))
                {
                    MuveletLista.Remove(muveletAzonosito);
                    break;
                }
            }

        }

        private void deleteButton(object subject, RoutedEventArgs e)
        {
            var clickedButton = subject as Button;
            string sectionName = clickedButton.Tag.ToString();

            List<Block> flowDocumentBlockList = uiFlowDocument.Blocks.ToList();
            uiFlowDocument.Blocks.Clear();

            foreach (var block in flowDocumentBlockList)
            {
                if (!block.Name.Equals(sectionName))
                {
                    uiFlowDocument.Blocks.Add(block);
                }
            }

            TaroltEljarasDictionary.Remove(sectionName);
            Kontener.Lista.Clear();


        }
        
    }
}