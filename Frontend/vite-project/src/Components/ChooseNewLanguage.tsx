import { useEffect, useState } from "react"
import { api } from "../axios/api";

type Language = {
    languageName: string;
    flag: string;
};

export default function ChooseNewLanguage() {

    const [languages, setLanguages] = useState<Language[]>([]);

    useEffect(() => {
        async function fetchLanguages() {
            const response = await api.get("languages");
            setLanguages(response.data);
        }
        fetchLanguages();
    }, [])

    return (
        <>
            <div> Which language do you want to learn? </div>
            {languages.map(l => <div> {l.languageName} </div>)}
        </>
    )
}