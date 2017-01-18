
var camelCaseTokenizer = function (obj) {
    var previous = '';
    return obj.toString().trim().split(/[\s\-]+|(?=[A-Z])/).reduce(function(acc, cur) {
        var current = cur.toLowerCase();
        if(acc.length === 0) {
            previous = current;
            return acc.concat(current);
        }
        previous = previous.concat(current);
        return acc.concat([current, previous]);
    }, []);
}
lunr.tokenizer.registerFunction(camelCaseTokenizer, 'camelCaseTokenizer')
var searchModule = function() {
    var idMap = [];
    function y(e) { 
        idMap.push(e); 
    }
    var idx = lunr(function() {
        this.field('title', { boost: 10 });
        this.field('content');
        this.field('description', { boost: 5 });
        this.field('tags', { boost: 50 });
        this.ref('id');
        this.tokenizer(camelCaseTokenizer);

        this.pipeline.remove(lunr.stopWordFilter);
        this.pipeline.remove(lunr.stemmer);
    });
    function a(e) { 
        idx.add(e); 
    }

    a({
        id:0,
        title:"TabEvent",
        content:"TabEvent",
        description:'',
        tags:''
    });

    a({
        id:1,
        title:"Resource",
        content:"Resource",
        description:'',
        tags:''
    });

    a({
        id:2,
        title:"Resource Attribute",
        content:"Resource Attribute",
        description:'',
        tags:''
    });

    a({
        id:3,
        title:"Resource Color",
        content:"Resource Color",
        description:'',
        tags:''
    });

    a({
        id:4,
        title:"Resource Animation",
        content:"Resource Animation",
        description:'',
        tags:''
    });

    a({
        id:5,
        title:"TabParser",
        content:"TabParser",
        description:'',
        tags:''
    });

    a({
        id:6,
        title:"Resource Layout",
        content:"Resource Layout",
        description:'',
        tags:''
    });

    a({
        id:7,
        title:"Resource Style",
        content:"Resource Style",
        description:'',
        tags:''
    });

    a({
        id:8,
        title:"IOnTabReselectListener",
        content:"IOnTabReselectListener",
        description:'',
        tags:''
    });

    a({
        id:9,
        title:"Resource Integer",
        content:"Resource Integer",
        description:'',
        tags:''
    });

    a({
        id:10,
        title:"IOnTabSelectListener",
        content:"IOnTabSelectListener",
        description:'',
        tags:''
    });

    a({
        id:11,
        title:"BottomBarTabType",
        content:"BottomBarTabType",
        description:'',
        tags:''
    });

    a({
        id:12,
        title:"BottomBarTab",
        content:"BottomBarTab",
        description:'',
        tags:''
    });

    a({
        id:13,
        title:"Resource Id",
        content:"Resource Id",
        description:'',
        tags:''
    });

    a({
        id:14,
        title:"BottomBarTabConfig",
        content:"BottomBarTabConfig",
        description:'',
        tags:''
    });

    a({
        id:15,
        title:"Resource String",
        content:"Resource String",
        description:'',
        tags:''
    });

    a({
        id:16,
        title:"BottomBar",
        content:"BottomBar",
        description:'',
        tags:''
    });

    a({
        id:17,
        title:"Resource Dimension",
        content:"Resource Dimension",
        description:'',
        tags:''
    });

    a({
        id:18,
        title:"BottomBarBadge",
        content:"BottomBarBadge",
        description:'',
        tags:''
    });

    a({
        id:19,
        title:"Resource Styleable",
        content:"Resource Styleable",
        description:'',
        tags:''
    });

    a({
        id:20,
        title:"Resource Boolean",
        content:"Resource Boolean",
        description:'',
        tags:''
    });

    a({
        id:21,
        title:"Resource Drawable",
        content:"Resource Drawable",
        description:'',
        tags:''
    });

    a({
        id:22,
        title:"BadgeContainer",
        content:"BadgeContainer",
        description:'',
        tags:''
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/TabEvent',
        title:"TabEvent",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/Resource',
        title:"Resource",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/Attribute',
        title:"Resource.Attribute",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/Color',
        title:"Resource.Color",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/Animation',
        title:"Resource.Animation",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/TabParser',
        title:"TabParser",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/Layout',
        title:"Resource.Layout",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/Style',
        title:"Resource.Style",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/IOnTabReselectListener',
        title:"IOnTabReselectListener",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/Integer',
        title:"Resource.Integer",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/IOnTabSelectListener',
        title:"IOnTabSelectListener",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/BottomBarTabType',
        title:"BottomBarTabType",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/BottomBarTab',
        title:"BottomBarTab",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/Id',
        title:"Resource.Id",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/BottomBarTabConfig',
        title:"BottomBarTabConfig",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/String',
        title:"Resource.String",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/BottomBar',
        title:"BottomBar",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/Dimension',
        title:"Resource.Dimension",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/BottomBarBadge',
        title:"BottomBarBadge",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/Styleable',
        title:"Resource.Styleable",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/Boolean',
        title:"Resource.Boolean",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/Drawable',
        title:"Resource.Drawable",
        description:""
    });

    y({
        url:'/BottomBarSharp/BottomBarSharp/api/BottomBarSharp/BadgeContainer',
        title:"BadgeContainer",
        description:""
    });

    return {
        search: function(q) {
            return idx.search(q).map(function(i) {
                return idMap[i.ref];
            });
        }
    };
}();
