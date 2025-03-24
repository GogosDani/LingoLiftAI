import { useEffect, useState } from "react"
import { api } from "../axios/api";

type Language = {
    languageName: string;
    flag: string;
    id: number;
};

export default function ChooseNewLanguage() {

    const [languages, setLanguages] = useState<Language[]>([]);

    useEffect(() => {
        async function fetchLanguages() {
            const response = await api.get("/api/language");
            setLanguages(response.data);
        }
        fetchLanguages();
    }, [])

    return (
        <>
            <div> Which language do you want to learn? </div>
            {languages.map(l => <div key={l.id}>
                <img src={l.flag} className="w-4" />
                <div> {l.languageName}  </div>
            </div>)}
        </>
    )
}