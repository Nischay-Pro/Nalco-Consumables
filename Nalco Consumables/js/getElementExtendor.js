(function () {
    if (!document.getElementsByClassName) {
        var indexOf = [].indexOf || function (prop) {
            for (var i = 0; i < this.length; i++) {
                if (this[i] === prop) return i;
            }
            return -1;
        };
        getElementsByClassName = function (className, context) {
            var elems = document.querySelectorAll ? context.querySelectorAll("." + className) : (function () {
                var all = context.getElementsByTagName("*"),
                    elements = [],
                    i = 0;
                for (; i < all.length; i++) {
                    if (all[i].className && (" " + all[i].className + " ").indexOf(" " + className + " ") > -1 && indexOf.call(elements, all[i]) === -1) elements.push(all[i]);
                }
                return elements;
            })();
            return elems;
        };
        document.getElementsByClassName = function (className) {
            return getElementsByClassName(className, document);
        };

        if (Element) {
            Element.prototype.getElementsByClassName = function (className) {
                return getElementsByClassName(className, this);
            };
        }
    }
})();