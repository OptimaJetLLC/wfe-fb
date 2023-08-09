import {DWKitForm} from "../optimajet-form";
import DocumentList from "../../forms/DocumentList.json";
import Document from "../../forms/Document.json";
import React, {useEffect, useState} from "react";

const DocumentListDataURL = `api/processes`;
const DocumentDataURL = `api/process/`;

const Form = {
  DocumentList: {
    name: "DocumentList",
    model: DocumentList,
    dataURL: DocumentListDataURL,
  },
  Document: {
    name: "Document",
    model: Document,
    dataURL: DocumentDataURL,
  }
}

const Event = {
  GridAdd: "gridAdd",
  GridEdit: "gridEdit",
  GridDelete: "gridDelete",
  GridRefresh: "gridRefresh",
  Save: "save",
  Redirect: "redirect"
}

export const FormViewer = () => {
  const [form, setForm] = useState(Form.DocumentList)
  const [data, setData] = useState([])
  const [row, setRow] = useState()
  const [int, setInt] = useState(0)

  useEffect(() => {
    const query = async () => {
      const url = row ? form.dataURL + row.id : form.dataURL
      const response = await fetch(url)
      const data = await response.json()
      setData(data)
    }

    query()
  }, [form, int]);

  return (
    <DWKitForm
      formName={form.name}
      data={data}
      model={form.model}
      eventFunc={(args) => {
        console.log('eventFunc', args);

        if (args.actions.includes(Event.GridEdit)) {
          setRow(args.parameters.row)
          setForm(Form.Document)
        }

        if (args.actions.includes(Event.Save)) {
          fetch(DocumentDataURL + row.id, {
            method: "PUT",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify(args.component.state.data)
          })
        }

        if (args.actions.includes(Event.GridDelete)) {
          const selectedIndexes = args.controlRef.state.selectedIndexes
          const ids = selectedIndexes.map(index => args.controlRef.state.items[index].id)

          fetch(DocumentDataURL, {
            method: "Delete",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify(ids)
          })
            .then(() => {
              setInt(prevState => prevState + 1)
            })
        }

        if (args.actions.includes(Event.GridAdd)) {
          fetch(DocumentDataURL, {
            method: "Post",
          })
            .then(() => {
              setInt(prevState => prevState + 1)
            })
        }

        if (args.actions.includes(Event.GridRefresh)) {
          setInt(prevState => prevState + 1)
        }

        if (args.actions.includes(Event.Redirect) && args.parameters.target === "back") {
          setRow(undefined)
          setForm(Form.DocumentList)
        }
      }}
      dataChanged={(form, {key, value}) => {
        console.log('dataChanged', 'form:', form, 'key:', key, 'value:', value);
      }}
    />
  )
}