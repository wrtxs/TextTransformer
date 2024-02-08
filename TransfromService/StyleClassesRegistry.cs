using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TransfromService
{
    public class StyleClassesRegistry
    {
        private Dictionary<string, StyleClass> _styleClasses;

        private HtmlNode _htmlRoot;

        public IDictionary<string, StyleClass> Items
        {
            get
            {
                if (_styleClasses == null)
                    PrepareStylesClassesDict();

                return _styleClasses;
            }
        }

        public StyleClassesRegistry(HtmlNode htmlRoot)
        {
            _htmlRoot = htmlRoot;
        }

        private void PrepareStylesClassesDict()
        {
            // Получаем все теги <style>
            if (_styleClasses != null)
                return;

            _styleClasses = new Dictionary<string, StyleClass>();

            var styleNodes = _htmlRoot.Descendants("style");

            // Проходим по всем тегам <style>
            foreach (var styleNode in styleNodes)
            {
                var styleContent = styleNode.InnerText;

                // Удаляем комментарии из кода
                styleContent = Regex.Replace(styleContent, @"\/\*(.*?)\*\/", "", RegexOptions.Singleline);


                var classRegex = new Regex(@"\.([\w-]+)\s*{([^}]*)}", RegexOptions.Singleline);
                var matches = classRegex.Matches(styleContent);

                // Проходим по всем классам стилей внутри конкретного тега <style>
                foreach (Match match in matches)
                {
                    var styleClassNames = match.Groups[1].Value.Trim().Split(','); // Получаем имена классов
                    var styleClassRawValue = match.Groups[2].Value.Trim();

                    var styleClassParams = new List<ClassStyleParameter>();
                    var styleClassParamsDict = new Dictionary<string, ClassStyleParameter>();

                    var pairValues = styleClassRawValue.Split(';');
                    foreach (var pairValue in pairValues)
                    {
                        var keyValue = pairValue.Split(':');
                        if (keyValue.Length == 2)
                        {
                            var styleClassParamName = keyValue[0].Trim().ToLower();
                            var styleClassParamValue = keyValue[1].Trim().ToLower();

                            var classStyleParameter =
                                new ClassStyleParameter(styleClassParamName, styleClassParamValue);

                            if (!styleClassParamsDict.ContainsKey(styleClassParamName))
                            {
                                styleClassParams.Add(classStyleParameter);
                                styleClassParamsDict.Add(styleClassParamName, classStyleParameter);
                            }
                            else
                            {
                                styleClassParams.Remove(styleClassParams.Find(_ => _.Name == styleClassParamName));
                                styleClassParams.Add(classStyleParameter);
                                styleClassParamsDict[styleClassParamName] = classStyleParameter;
                            }
                        }
                    }

                    // Добавляем стили для каждого класса в _styleClasses
                    foreach (var className in styleClassNames)
                    {
                        var trimmedClassName = className.Trim();

                        if (!_styleClasses.ContainsKey(trimmedClassName))
                        {
                            _styleClasses.Add(trimmedClassName, new StyleClass(trimmedClassName, styleClassRawValue, styleClassParams, styleClassParamsDict));
                        }
                        else
                        {
                            _styleClasses[trimmedClassName].SplitParams(styleClassParams);
                        }
                    }
                }
            }
        }

        public class StyleClass
        {
            public string Name { get; }

            private readonly List<ClassStyleParameter> _parameters;
            public IEnumerable<ClassStyleParameter> Parameters => _parameters;
            public IDictionary<string, ClassStyleParameter> ParametersDict { get; }
            public string RawParametersValue { get; }

            public StyleClass(string name, string rawParametersValue, List<ClassStyleParameter> parameters = null, IDictionary<string, ClassStyleParameter> parametersDict = null)
            {
                Name = name;
                RawParametersValue = rawParametersValue;
                _parameters = parameters ?? new List<ClassStyleParameter>();
                ParametersDict = parametersDict ?? new Dictionary<string, ClassStyleParameter>();
            }

            public void SplitParams(List<ClassStyleParameter> styleClassParams)
            {
                foreach (var newParam in styleClassParams)
                {
                    if (!ParametersDict.ContainsKey(newParam.Name))
                    {
                        _parameters.Add(newParam);
                        ParametersDict.Add(newParam.Name, newParam);
                    }
                    else
                    {
                        _parameters.Remove(_parameters.Find(_ => _.Name == newParam.Name));
                        _parameters.Add(newParam);
                        ParametersDict[newParam.Name] = newParam;
                    }
                }
            }
        }

        public class ClassStyleParameter
        {
            public ClassStyleParameter(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; }
            public string Value { get; }

            public override string ToString()
            {
                return $"{Name}: {Value};";
            }
        }
    }
}