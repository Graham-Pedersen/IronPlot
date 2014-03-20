﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace IRLanguage.ParenMatch
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IViewTaggerProvider))]
    [ContentType("IronRacket")]
    [TagType(typeof(TextMarkerTag))]
    public sealed class IRTaggerProvider : IViewTaggerProvider
    {
        [Import]
        public IClassifierAggregatorService AggregatorService;

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null)
                return null;

            var aggregator = AggregatorService.GetClassifier(buffer);
            var pairs = new KeyValuePair<char, char>[]
                {
                    new KeyValuePair<char, char>('(', ')'),
                    new KeyValuePair<char, char>('{', '}'),
                    new KeyValuePair<char, char>('[', ']')
                };
            return new BraceMatchingTagger(textView, buffer, aggregator, pairs) as ITagger<T>;
        }
    }
}
