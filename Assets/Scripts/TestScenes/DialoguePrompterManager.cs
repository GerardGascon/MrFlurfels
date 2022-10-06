using Assets.Scripts.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.TestScenes
{
#if UNITY_EDITOR
    [CustomEditor(typeof(DialoguePrompterManager))]
    public class DialoguePrompterManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DialoguePrompterManager myScript = (DialoguePrompterManager) target;
            if (GUILayout.Button("Generate Readable Text"))
            {
                myScript.GenerateReadableText();
                Debug.Log("Readable Text Generated.");
            }
        }
    }
#endif

    public class DialoguePrompterManager : MonoBehaviour
    {
        public Action OnNextText;
        public Action OnFinish;

        private const string NombrePerro = "Mr. Flurfels",
            NombreGato                   = "Catarina",
            NombreZorro                  = "Felipe",
            NombreHipo                   = "Hipólito",
            NombreRana                   = "Justiniana",
            NombreSapo                   = "Begoña",
            NombreOso                    = "Isidoro",
            NombrePajaro                 = "Steve";

        [SerializeField] private TextAsset       _xmlRawFile;
        [SerializeField] private CharacterImages _images;

        public readonly Dictionary<string, bool> _gameVariables =
            new Dictionary<string, bool>();

        public List<object> Blocked = new List<object>();

        private        float          _originalTextSize;
        private static PotatoDialogue _dialogue;

        public bool PlayingDialogue { get; private set; }

        private Animator anim;

        // Start is called before the first frame update
        private void Start()
        {
            _originalTextSize = PotatoDialogueManager.Instance.DialogueItems.TextBox.fontSize;
            anim              = GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Submit") && _dialogue != null && Blocked.Count == 0)
            {
                OnNextText?.Invoke();

                if (PotatoDialogueManager.Instance.Dialogue(_dialogue, "blob1", "blob2") ||
                    (Notepad.instance != null && Notepad.instance.opened)) return;
                _dialogue = null;
                anim.SetTrigger("Out");
                PlayingDialogue = false;
                OnFinish?.Invoke();

                PlayerController player           = FindObjectOfType<PlayerController>();
                if (player) player.talkingBlocked = true;
            }
        }

        public void PlayDialogue(int a_dialogueID)
        {
            if (Blocked.Count != 0) return;
            _dialogue = GetDialogueByID(a_dialogueID);
            PotatoDialogueManager.Instance.Dialogue(_dialogue, "blob1", "blob2");
            anim.SetTrigger("In");

            PlayingDialogue = true;
        }

        private PotatoDialogue GetDialogueByID(int a_dialogueID)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(new StringReader(_xmlRawFile.text));
            string xmlPathPattern = $"dialogues/dialogue[@id=\"{a_dialogueID}\"]";

            XmlNode node = xmlDoc.SelectSingleNode(xmlPathPattern);

            if (node == null)
                Debug.LogError($"No se ha encontrado el diálogo con la id {a_dialogueID}.");

            List<PotatoSentence> dialogueBoxes = new List<PotatoSentence>();
            ProcessNodeChilds(node, dialogueBoxes);

            PotatoDialogue dialogue = new PotatoDialogue {Sentences = dialogueBoxes.ToArray()};
            return dialogue;
        }


        private void ProcessNodeChilds(XmlNode a_node, List<PotatoSentence> a_dialogue)
        {
            bool processingCondition = false;
            bool conditionScaped     = false;

            foreach (XmlNode childNode in a_node.ChildNodes)
            {
                if (processingCondition && childNode.Name != "elif" && childNode.Name != "else")
                {
                    processingCondition = false;
                    conditionScaped     = false;
                }

                if (childNode.Name == "if")
                {
                    processingCondition = true;
                    conditionScaped     = ProcessIf(a_dialogue, childNode);
                }
                else if (childNode.Name == "elif")
                {
                    if (!conditionScaped)
                        conditionScaped = ProcessIf(a_dialogue, childNode);
                }
                else if (childNode.Name == "else")
                {
                    if (!conditionScaped)
                        ProcessNodeChilds(childNode, a_dialogue);
                }
                else if (childNode.Name == "set")
                {
                    if (childNode.Attributes != null)
                    {
                        string variable = childNode.Attributes["variable"].Value;
                        string value    = childNode.InnerText;
                        bool   parsedValue;

                        switch (value)
                        {
                            case "1":
                                parsedValue = true;
                                break;
                            case "0":
                                parsedValue = false;
                                break;
                            default:
                                Debug.LogError(
                                    "Valor incorrecto para el seteo de la variable " +
                                    variable + ": " + value);
                                return;
                        }

                        if (_gameVariables.ContainsKey(variable))
                            _gameVariables[variable] = parsedValue;
                        else
                            _gameVariables.Add(variable, parsedValue);
                    }
                    else
                    {
                        Debug.LogError("Encontrado set sin variable asignada:" +
                            childNode.OuterXml);
                    }
                }
                else if (childNode.Name == "callback")
                {
                    Debug.Log("Se callan");
                }
                else if (childNode.Name.StartsWith("p"))
                {
                    Sprite img;
                    if (childNode.Name[1] == '0')
                        img = _images.P0;
                    else if (childNode.Name[1] == '1')
                        img = _images.P1;
                    else if (childNode.Name[1] == '2')
                        img = _images.P2;
                    else if (childNode.Name[1] == '3')
                        img = _images.P3;
                    else
                        img = _images.P4;

                    AddCharacterSentence(a_dialogue, childNode, NombrePerro, img);
                }
                else if (childNode.Name.StartsWith("g"))
                {
                    Sprite img;
                    if (childNode.Name[1] == '0')
                        img = _images.G0;
                    else if (childNode.Name[1] == '1')
                        img = _images.G1;
                    else if (childNode.Name[1] == '2')
                        img = _images.G2;
                    else if (childNode.Name[1] == '3')
                        img = _images.G3;
                    else if (childNode.Name[1] == '4')
                        img = _images.G4;
                    else if (childNode.Name[1] == '5')
                        img = _images.G5;
                    else if (childNode.Name[1] == '6')
                        img = _images.G6;
                    else
                        img = _images.G7;

                    AddCharacterSentence(a_dialogue, childNode, NombreGato, img);
                }
                else if (childNode.Name.StartsWith("s"))
                {
                    AddCharacterSentence(a_dialogue, childNode, NombreSapo, _images.S0);
                }
                else if (childNode.Name.StartsWith("r"))
                {
                    AddCharacterSentence(a_dialogue, childNode, NombreRana, _images.R0);
                }
                else if (childNode.Name.StartsWith("o"))
                {
                    AddCharacterSentence(a_dialogue, childNode, NombreOso, _images.O0);
                }
                else if (childNode.Name.StartsWith("z"))
                {
                    AddCharacterSentence(a_dialogue, childNode, NombreZorro, _images.Z0);
                }
                else if (childNode.Name.StartsWith("h"))
                {
                    Sprite img;
                    if (childNode.Name[1] == '0')
                        img = _images.H0;
                    else if (childNode.Name[1] == '1')
                        img = _images.H1;
                    else if (childNode.Name[1] == '2')
                        img = _images.H2;
                    else if (childNode.Name[1] == '3')
                        img = _images.H3;
                    else
                        img = _images.H4;

                    AddCharacterSentence(a_dialogue, childNode, NombreHipo, img);
                }
                else if (childNode.Name.StartsWith("c"))
                {
                    Sprite img;
                    if (childNode.Name[1] == '0')
                        img = _images.C0;
                    else if (childNode.Name[1] == '1')
                        img = _images.C1;
                    else if (childNode.Name[1] == '2')
                        img = _images.C2;
                    else if (childNode.Name[1] == '3')
                        img = _images.C3;
                    else if (childNode.Name[1] == '4')
                        img = _images.C4;
                    else if (childNode.Name[1] == '5')
                        img = _images.C5;
                    else
                        img = _images.C6;

                    AddCharacterSentence(a_dialogue, childNode, NombrePajaro, img);
                }
                else if (childNode.Name.StartsWith("i"))
                {
                    a_dialogue.Add(new PotatoSentence
                    {
                        DisplayName    = false,
                        CharacterName  = "",
                        Sentence       = ProcessText(childNode.InnerText),
                        Side           = SentenceSide.None,
                        CharacterImage = null
                    });
                }
                else if (childNode.Name == "#comment")
                {
                    Debug.LogWarning("Te has dejado un comentario. Yo te aviso.");
                }
                else
                {
                    Debug.LogError("Unrecognized command: " + childNode.Name);
                }
            }
        }

        private bool ProcessIf(List<PotatoSentence> a_dialogue, XmlNode a_childNode)
        {
            if (a_childNode.Attributes != null)
            {
                string condition = a_childNode.Attributes["condition"].Value;

                bool expectedValue;
                if (condition.StartsWith("!"))
                {
                    expectedValue = false;
                    condition     = condition.Remove(0, 1);
                }
                else
                {
                    expectedValue = true;
                }

                if (_gameVariables.ContainsKey(condition) &&
                    _gameVariables[condition] == expectedValue ||
                    _gameVariables.ContainsKey(condition) == expectedValue)
                {
                    ProcessNodeChilds(a_childNode, a_dialogue);
                    return true;
                }
            }
            else
            {
                Debug.LogError("Encontrado if sin condición:" + a_childNode.OuterXml);
            }

            return false;
        }

        private void AddCharacterSentence(
            List<PotatoSentence> a_dialogue,
            XmlNode              a_childNode,
            string               a_characterName,
            Sprite               a_sprite)
        {
            bool explainedByCat = a_childNode.Name.Length > 2 && a_childNode.Name[2] == 'g';

            a_dialogue.Add(new PotatoSentence
            {
                DisplayName   = true,
                CharacterName = explainedByCat ? NombreGato : a_characterName,
                Sentence      = ProcessText(a_childNode.InnerText),
                Side = a_childNode.Name.Length > 2 && a_childNode.Name[2] == 'i'
                    ? SentenceSide.Left
                    : SentenceSide.Right,
                CharacterImage = a_sprite
            });
        }

        private string ProcessText(string a_text)
        {
            a_text = a_text.Replace("\\n", "\n");

            a_text = a_text.Replace("[p]", $"<b>[c=p]{NombrePerro}</color></b>");
            a_text = a_text.Replace("[g]", $"<b>[c=g]{NombreGato}</color></b>");
            a_text = a_text.Replace("[s]", $"<b>[c=s]{NombreSapo}</color></b>");
            a_text = a_text.Replace("[r]", $"<b>[c=r]{NombreRana}</color></b>");
            a_text = a_text.Replace("[o]", $"<b>[c=o]{NombreOso}</color></b>");
            a_text = a_text.Replace("[z]", $"<b>[c=z]{NombreZorro}</color></b>");
            a_text = a_text.Replace("[h]", $"<b>[c=h]{NombreHipo}</color></b>");
            a_text = a_text.Replace("[c]", $"<b>[c=c]{NombrePajaro}</color></b>");

            a_text = a_text.Replace("[c=p]", "<color=#880808>");
            a_text = a_text.Replace("[c=g]", "<color=#880808>");
            a_text = a_text.Replace("[c=s]", "<color=#880808>");
            a_text = a_text.Replace("[c=r]", "<color=#880808>");
            a_text = a_text.Replace("[c=o]", "<color=#880808>");
            a_text = a_text.Replace("[c=z]", "<color=#880808>");
            a_text = a_text.Replace("[c=h]", "<color=#880808>");
            a_text = a_text.Replace("[c=c]", "<color=#880808>");

            a_text = a_text.Replace("[/c]", "</color>");

            a_text = a_text.Replace("[?]", "<b><color=#982d4b>");
            a_text = a_text.Replace("[/?]", "</color></b>");

            a_text = a_text.Replace("[*]", "<b><color=#982d4b><snd:pregunta>");
            a_text = a_text.Replace("[/*]", "</color></b>");

			a_text = a_text.Replace("[ds]", "<stopmsc:musica_juego,1><playmsc:musica_steve,1>");
			a_text = a_text.Replace("[/ds]", "<stopmsc:musica_steve,1><playmsc:musica_juego,1>");

			a_text = a_text.Replace("[f]", "<stopmsc:musica_juego,1><playmsc:final_bueno,1>");
			a_text = a_text.Replace("[/f]", "<stopmsc:final_bueno,1><playmsc:musica_juego,1>");

			a_text = a_text.Replace("[i]", "<i>");
            a_text = a_text.Replace("[/i]", "</i>");

            a_text = a_text.Replace("[b]", "<b>");
            a_text = a_text.Replace("[/b]", "</b>");

            a_text = a_text.Replace("[ +]", "<cspace=0.12em>");
            a_text = a_text.Replace("[/ +]", "</cspace>");

            a_text = a_text.Replace("[ -]", "<cspace=-0.12em>");
            a_text = a_text.Replace("[/ -]", "</cspace>");

            a_text = a_text.Replace("[sp-]", "<sp:50>");
            a_text = a_text.Replace("[sp--]", "<sp:25>");

            a_text = a_text.Replace("[xl]", $"<size={(int) (_originalTextSize * 1.12f)}>");
            a_text = a_text.Replace("[/xl]", "</size>");

            a_text = a_text.Replace("[xxl]", $"<size={(int) (_originalTextSize * 1.25f)}>");
            a_text = a_text.Replace("[/xxl]", "</size>");

            a_text = a_text.Replace("[xxxl]", $"<size={(int) (_originalTextSize * 1.5f)}>");
            a_text = a_text.Replace("[/xxxl]", "</size>");

            a_text = a_text.Replace("[xxxxl]", $"<size={(int) (_originalTextSize * 2f)}>");
            a_text = a_text.Replace("[/xxxxl]", "</size>");

            a_text = a_text.Replace("[xs]", $"<size={(int) (_originalTextSize * 0.88f)}>");
            a_text = a_text.Replace("[/xs]", "</size>");

            a_text = a_text.Replace("[xxs]", $"<size={(int) (_originalTextSize * 0.75f)}>");
            a_text = a_text.Replace("[/xxs]", "</size>");

            a_text = a_text.Replace("[xxxs]", $"<size={(int) (_originalTextSize * 0.5f)}>");
            a_text = a_text.Replace("[/xxxs]", "</size>");

            a_text = a_text.Replace("[~]", "<anim:wobble>");
            a_text = a_text.Replace("[/~]", "</anim>");

            a_text = a_text.Replace("[~~]", "<anim:wave>");
            a_text = a_text.Replace("[/~~]", "</anim>");

            a_text = a_text.Replace("[~~~]", "<anim:rainbow>");
            a_text = a_text.Replace("[/~~~]", "</anim>");

            a_text = a_text.Replace("[~~~~]", "<anim:shake>");
            a_text = a_text.Replace("[/~~~~]", "</anim>");

            a_text = a_text.Replace("[.]", "<p:tiny>");
            a_text = a_text.Replace("[..]", "<p:short>");
            a_text = a_text.Replace("[...]", "<p:normal>");
            a_text = a_text.Replace("[....]", "<p:long>");
            a_text = a_text.Replace("[.....]", "<p:read>");

            return a_text;
        }

        [Serializable]
        public struct CharacterImages
        {
            public Sprite P0;
            public Sprite P1;
            public Sprite P2;
            public Sprite P3;
            public Sprite P4;
            public Sprite G0;
            public Sprite G1;
            public Sprite G2;
            public Sprite G3;
            public Sprite G4;
            public Sprite G5;
            public Sprite G6;
            public Sprite G7;
            public Sprite Z0;
            public Sprite S0;
            public Sprite C0;
            public Sprite C1;
            public Sprite C2;
            public Sprite C3;
            public Sprite C4;
            public Sprite C5;
            public Sprite C6;
            public Sprite H0;
            public Sprite H1;
            public Sprite H2;
            public Sprite H3;
            public Sprite H4;
            public Sprite O0;
            public Sprite R0;
        }

        public void GenerateReadableText()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(new StringReader(_xmlRawFile.text));
            string xmlPathPattern = "dialogues";

            StringWriter sw = new StringWriter();

            XmlNode node = xmlDoc.SelectSingleNode(xmlPathPattern);

            string text = node.InnerXml;

            text = Regex.Replace(text, "(?:<dialogue id=\")(\\d*?)(?:\">)",
                a_match => $"\n\nDIALOGO {a_match.Groups[1]}: ");

            text = Regex.Replace(text, "(?:<if condition=\"!)(.*?)(?:\">)",
                a_match => $"\n--------- SI NO {a_match.Groups[1]} ---------\n");

            text = Regex.Replace(text, "(?:<if condition=\")(.*?)(?:\">)",
                a_match => $"\n--------- SI {a_match.Groups[1]} ---------\n");

            text = Regex.Replace(text, "(<\\/if>)",
                a_match => $"\n--------- FIN SI ---------\n");

            text = Regex.Replace(text, "(<else>)",
                a_match => "\n--------- EN CASO CONTRARIO ---------\n");

            text = Regex.Replace(text, "(<\\/else>)",
                a_match => "\n--------- FIN EN CASO CONTRARIO ---------\n");

            text = Regex.Replace(text, "(?:<set variable=\")(.*?)(?:\">)(.*?)(?:<\\/set>)",
                a_match =>
                {
                    string state;
                    switch (a_match.Groups[2].Value)
                    {
                        case "1":
                            state = "ACTIVADA";
                            break;
                        case "0":
                            state = "DESACTIVADA";
                            break;
                        default:
                            state = "FALLANDO";
                            break;
                    }

                    return $"\n!!! AHORA {a_match.Groups[1]} ESTÁ {state} !!!\n";
                });

            text = Regex.Replace(text, "<p.*?>", "\nFLURFELS:   ");
            text = Regex.Replace(text, "<g.*?>", "\nCATARINA:   ");
            text = Regex.Replace(text, "<c.*?>", "\nSTEVE:      ");
            text = Regex.Replace(text, "<o.*?>", "\nISIDORO:    ");
            text = Regex.Replace(text, "<r.*?>", "\nJUSTINIANA: ");
            text = Regex.Replace(text, "<s.*?>", "\nBEGOÑA:     ");
            text = Regex.Replace(text, "<z.*?>", "\nFELIPE:     ");
            text = Regex.Replace(text, "<h.*?>", "\nHIPÓLITO:   ");


            text = Regex.Replace(text, "(\\[.*?\\])", string.Empty);
            text = Regex.Replace(text, "(<.*?>)", string.Empty);

            File.WriteAllText("ReadableScript.txt", text);

            //foreach (XmlNode childNode in node.ChildNodes)
            //{
            //    sw.Write("Conversaci�n: ");
            //}
        }
    }
}