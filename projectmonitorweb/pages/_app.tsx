import { AppProps } from 'next/app';
import Head from 'next/head';
import 'bootstrap/dist/css/bootstrap.min.css';
import '../css/global.css';
import '../css/dashboard.css';

export default function App({ Component, pageProps }: AppProps) {
  if(process.env.NODE_ENV === 'development' ){
    process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
  }

  return (
    <>
        <Head>
            <title>Dashboard</title>
        </Head>

        <Component {...pageProps} />
    </>
  );
}