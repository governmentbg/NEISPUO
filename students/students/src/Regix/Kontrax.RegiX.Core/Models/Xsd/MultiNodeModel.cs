using System.Collections.Generic;
using System.Linq;

namespace Kontrax.RegiX.Core.TestStandard.Models
{
    public class MultiNodeModel
    {
        public string QName { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string TypeName { get; set; }

        public string TypeTitle { get; set; }

        public int Min { get; set; }

        public int? Max { get; set; }

        public string MultiplicitySymbol { get; set; }

        public KeyElementViewModel KeyElement { get; set; }

        public void SetKeyElements(Dictionary<string, KeyElementViewModel> keyElements)
        {
            InferLocalName();  // Кръпка след POST.

            KeyElementViewModel keyElement = null;
            if (Name != null)
            {
                keyElements.TryGetValue(Name, out keyElement);
            }
            KeyElement = keyElement;

            if (Instances != null)
            {
                foreach (DataNodeModel instance in Instances)
                {
                    instance.KeyTypeCodes = keyElement?.KeyTypes.Select(t => t.Code);
                    if (instance.SupportsChildren)
                    {
                        foreach (MultiNodeModel child in instance.Children)
                        {
                            child.SetKeyElements(keyElements);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Възстановява краткото име преди по него да се търси KeyElement, защото то не се POST-ва и може да не е заредено в момента.
        /// TODO: Ако преди Create/Update се възстановяваха всички метаданни, това нямаше да е необходимо.
        /// </summary>
        private void InferLocalName()
        {
            if (Name == null && QName != null)
            {
                int colonPos = QName.LastIndexOf(":");
                Name = colonPos >= 0 ? QName.Substring(colonPos + 1) : QName;
            }
        }

        /// <summary>
        /// Връща стойността на първия елемент/инстанция, която съдържа ЕГН, ЛНЧ или ЕИК и стойността не е празна.
        /// Информацията кои полета съдържат ЕГН, ЛНЧ или ЕИК (KeyElement) трябва да е заредена предварително.
        /// </summary>
        public string PersonOrCompanyId
        {
            get
            {
                string personOrCompanyId = null;
                if (Instances != null)
                {
                    if (KeyElement != null)
                    {
                        // Този елемент е предвиден за ЕГН, ЛНЧ или ЕИК.
                        personOrCompanyId = (
                            from i in Instances
                            select i.Value
                        ).FirstOrDefault(v => !string.IsNullOrEmpty(v));
                    }

                    if (personOrCompanyId == null)
                    {
                        personOrCompanyId = (
                            from i in Instances
                            where i.SupportsChildren
                            from c in i.Children
                            select c.PersonOrCompanyId
                        ).FirstOrDefault(id => id != null);
                    }
                }
                return personOrCompanyId;
            }
        }

        private string[] _enumValues;

        public string[] EnumValues
        {
            get
            {
                return _enumValues;
            }
            set
            {
                _enumValues = value;
                // MultiNodeModel.EnumValues не се ползва във view-та. Списъкът със стойности се разкопира
                // по всички инстанции, за да може да се ползва editor template за инстанцията.
                //TypeName се използва в editor template за визуализация на специални типове - месец, дата и т.н.
                if (Instances != null)
                {
                    foreach (DataNodeModel instance in Instances)
                    {
                        instance.EnumValues = _enumValues;
                        instance.TypeName = TypeName;
                    }
                }
            }
        }

        public DataNodeModel[] Instances { get; set; }

        public bool ValidateRequiredFields()
        {
            bool isValid = true;
            if (Instances != null)
            {
                foreach (DataNodeModel instance in Instances)
                {
                    if (instance.IsRequired)
                    {
                        if (instance.IsEmpty)
                        {
                            if (instance.SupportsChildren)
                            {
                                isValid = false;
                                instance.ErrorMessage = $"Секцията е задължителна. Моля попълнете я.";
                            }
                            else if (instance.SupportsValue)
                            {
                                isValid = false;
                                string verb = instance.EnumValues != null && instance.EnumValues.Length > 0 ? "изберете" : "въведете";
                                instance.ErrorMessage = $"Полето е задължително. Моля {verb} стойност.";
                            }
                        }

                        if (instance.SupportsChildren)
                        {
                            foreach (MultiNodeModel child in instance.Children)
                            {
                                isValid &= child.ValidateRequiredFields();
                            }
                        }
                    }
                }
            }
            return isValid;
        }

        public void CopyViewDataFrom(MultiNodeModel fromModel, List<string> errors)
        {
            if (QName != fromModel.QName)
            {
                errors.Add($"Елементът с qualified name {QName} вече не съществува или е на друга позиция.");
                return;
            }
            Name = fromModel.Name;
            Title = fromModel.Title;
            TypeName = fromModel.TypeName;
            TypeTitle = fromModel.TypeTitle;
            Min = fromModel.Min;
            Max = fromModel.Max;
            MultiplicitySymbol = fromModel.MultiplicitySymbol;
            KeyElement = fromModel.KeyElement;
            EnumValues = fromModel.EnumValues;

            DataNodeModel[] fromInstances = fromModel.Instances;
            // Поделементите на всички инстанции са еднакви, затова се ползват поделементите на първата инстанция.
            MultiNodeModel[] fromChildren = fromInstances?.FirstOrDefault()?.Children;

            if (Instances != null)
            {
                for (int i = 0; i < Instances.Length; i++)
                {
                    DataNodeModel toInstance = Instances[i];

                    // Първите min броя инстанции са задължителни, затова този флаг се копира от съответната по номер
                    // from-инстанция (а не от първата, както се прави с поделементите).
                    if (fromInstances != null && i < fromInstances.Length)
                    {
                        toInstance.IsRequired = fromInstances[i].IsRequired;
                        toInstance.KeyTypeCodes = fromInstances[i].KeyTypeCodes;
                    }

                    MultiNodeModel[] toChildren = toInstance.Children;
                    if (toChildren != null)
                    {
                        if (fromChildren == null)
                        {
                            errors.Add($"Елементът с qualified name {QName} вече няма поделементи.");
                            return;
                        }
                        if (toChildren.Length != fromChildren.Length)
                        {
                            errors.Add($"Елементът с qualified name {QName} вече има друг брой поделементи.");
                            return;
                        }
                        for (int j = 0; j < toChildren.Length; j++)
                        {
                            toChildren[j].CopyViewDataFrom(fromChildren[j], errors);
                        }
                    }
                }
            }
        }

        private RequestBaseModel _request;

        public RequestBaseModel Request
        {
            get
            {
                return _request;
            }
            set
            {
                _request = value;
                if (Instances != null)
                {
                    foreach (DataNodeModel instance in Instances)
                    {
                        instance.Request = value;
                    }
                }
            }
        }
    }
}
