const path = require('path');

module.exports = {
    entry: [
        './datepicker/ts/jinget.dp.ts'
    ],
    resolve: {
        extensions: ['.ts', '.js'],
    },
    devtool: 'inline-source-map',
    mode: 'production',
    output: {
        library: 'jinget_dp',
        path: path.resolve(__dirname, './datepicker'),
        filename: 'jinget.dp.js'
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                use: [
                    {
                        loader: 'expose-loader',
                        options: {
                            exposes: ['jinget_dp'],
                        },
                    },
                    {
                        loader: 'ts-loader',
                    }
                ],
                exclude: /node_modules/,
            },
        ],
    },
    externals: {
        bootstrap: 'bootstrap',
    },
    target: 'web',
    //exit webpack after build finished
    watch: false,
    performance: {
        maxEntrypointSize: 512000,
        maxAssetSize: 512000
    },
};