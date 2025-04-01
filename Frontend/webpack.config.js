const path = require('path');  
const HtmlWebpackPlugin = require('html-webpack-plugin');  

module.exports = {  
    mode: 'development', // або 'production'  
    entry: './src/index.ts', // Ваша точка входу  
    output: {  
        filename: 'bundle.js', // Ім'я вихідного файлу  
        path: path.resolve(__dirname, 'dist'), // Директорія для виходу  
        clean: true, // Очищення директорії dist перед кожною збіркою  
    },  
    module: {  
        rules: [  
            {  
                test: /\.js$/, // Обробка .js файлів  
                exclude: /node_modules/, // Ігнорувати node_modules  
                use: {  
                    loader: 'babel-loader', // Використання Babel для трансляції  
                    options: {  
                        presets: ['@babel/preset-env'], // Пресет для ES6+  
                    },  
                },  
            },  
        ],  
    },  
    plugins: [  
        new HtmlWebpackPlugin({  
            template: './src/index.html', // Шаблон HTML-файлу  
        }),  
    ],  
    devtool: 'inline-source-map', // Генерація source maps  
    devServer: {
        static: {
          directory: path.join(__dirname, 'dist'), // ✅ Правильний варіант
        },
        port: 3000,
      }
      ,  
};  